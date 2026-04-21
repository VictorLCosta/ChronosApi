using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Application.Common.Exceptions;
using Application.Common.Identity.Services;
using Application.Common.Interfaces;

using Infrastructure.Persistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Identity.Services;

public class IdentityService(
    ApplicationDbContext dbContext,
    IRequestContext requestContext,
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager,
    ILogger<IdentityService> logger,
    TimeProvider timeProvider
) : IIdentityService
{
    public async Task<(string Subject, IEnumerable<Claim> Claims)?> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(password);

        var user = await FindAndValidateUserByCredentialsAsync(email, password);

        var claims = await BuildUserClaimsAsync(user, ct);

        return (user.Id, claims);
    }

    public async Task StoreRefreshTokenAsync(string subject, string refreshToken, DateTime expiresAtUtc, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        var user = await userManager.FindByIdAsync(subject);
        if (user is null)
        {
            throw new UnauthorizedException();
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var hashedRefreshToken = HashRefreshToken(refreshToken);

        var existingActiveTokens = await dbContext.RefreshTokens
            .Where(rt => rt.UserId == user.Id && rt.RevokedAtUtc == null && rt.ExpiresAtUtc > now)
            .OrderByDescending(rt => rt.CreatedAtUtc)
            .ToListAsync(ct);

        foreach (var token in existingActiveTokens.Skip(4))
        {
            token.RevokedAtUtc = now;
            token.RevokedReason = "Active session limit reached.";
        }

        var entry = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = hashedRefreshToken,
            FamilyId = Guid.NewGuid().ToString("N"),
            CreatedAtUtc = now,
            ExpiresAtUtc = expiresAtUtc,
            IpAddress = requestContext.IpAddress,
            UserAgent = requestContext.UserAgent
        };

        await dbContext.RefreshTokens.AddAsync(entry, ct);

        await PruneExpiredRefreshTokensAsync(subject, now, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<(string Subject, IEnumerable<Claim> Claims)?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        var hashedRefreshToken = HashRefreshToken(refreshToken);

        var storedToken = await dbContext.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.TokenHash == hashedRefreshToken, ct);

        if (storedToken is null)
        {
            logger.LogInformation("Refresh token validation failed: token hash not found.");
            return null;
        }

        if (storedToken.User is null || !storedToken.User.IsActive)
        {
            logger.LogInformation("Refresh token validation failed for user {UserId}. User inactive or unavailable.", storedToken.UserId);
            return null;
        }

        if (storedToken.IsRevoked)
        {
            await RevokeTokenFamilyAsync(storedToken.UserId, storedToken.FamilyId, "Detected refresh token reuse.", ct);
            logger.LogWarning("Revoked refresh token family {FamilyId} for user {UserId} after reuse attempt.", storedToken.FamilyId, storedToken.UserId);
            return null;
        }

        if (storedToken.IsExpired)
        {
            logger.LogInformation("Refresh token validation failed for user {UserId}. Token expired at {ExpiresAtUtc}.", storedToken.UserId, storedToken.ExpiresAtUtc);
            return null;
        }

        storedToken.LastUsedAtUtc = timeProvider.GetUtcNow().UtcDateTime;
        await dbContext.SaveChangesAsync(ct);

        var claims = await BuildUserClaimsAsync(storedToken.User, ct);

        return (storedToken.User.Id, claims);
    }

    private async Task<AppUser> FindAndValidateUserByCredentialsAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email.Trim().Normalize());
        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedException();
        }

        if (!await signInManager.CanSignInAsync(user))
        {
            throw new UnauthorizedException("User is not allowed to sign in.");
        }

        if (!await userManager.CheckPasswordAsync(user, password))
        {
            throw new UnauthorizedException();
        }

        return user;
    }

    private async Task<List<Claim>> BuildUserClaimsAsync(AppUser user, CancellationToken ct)
    {
        var claims = CreateBasicClaims(user);
        await AddRoleClaimsAsync(claims, user, ct);

        return claims;
    }

    private async Task AddRoleClaimsAsync(List<Claim> claims, AppUser user, CancellationToken ct)
    {
        var directRoles = await userManager.GetRolesAsync(user);

        claims.AddRange(directRoles.Select(r => new Claim(ClaimTypes.Role, r)));
    }

    private async Task PruneExpiredRefreshTokensAsync(string userId, DateTime now, CancellationToken ct)
    {
        var staleTokens = await dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && (rt.ExpiresAtUtc <= now || rt.RevokedAtUtc != null))
            .ToListAsync(ct);

        if (staleTokens.Count > 0)
        {
            dbContext.RefreshTokens.RemoveRange(staleTokens);
        }
    }

    private async Task RevokeTokenFamilyAsync(string userId, string familyId, string reason, CancellationToken ct)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var familyTokens = await dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.FamilyId == familyId && rt.RevokedAtUtc == null)
            .ToListAsync(ct);

        foreach (var token in familyTokens)
        {
            token.RevokedAtUtc = now;
            token.RevokedReason = reason;
        }

        if (familyTokens.Count > 0)
        {
            await dbContext.SaveChangesAsync(ct);
        }
    }

    private static string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(bytes);
    }

    private static List<Claim> CreateBasicClaims(AppUser user) =>
    [
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Email, user.Email!),
        new(ClaimTypes.Name, user.Name ?? string.Empty),
        new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
        new(ClaimTypes.Surname, user.UserName ?? string.Empty),
        new("image_url", user.AvatarUrl?.ToString() ?? string.Empty)
    ];
}