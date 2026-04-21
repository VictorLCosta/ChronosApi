using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Application.Common.Identity.Models;
using Application.Common.Identity.Services;

using Infrastructure.Auth;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity.Services;

public class TokenService(IOptions<JwtOptions> options, TimeProvider timeProvider, ILogger<TokenService> logger) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    public Task<TokenResponse> GetTokenAsync(string subject, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var accessTokenExpiry = now.AddMinutes(_options.DurationInMinutes);

        var jwtToken = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            expires: accessTokenExpiry,
            claims: claims,
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = now.AddDays(_options.RefreshTokenDurationInDays);

        logger.LogInformation("Issued JWT for subject {Subject}", subject);

        var response = new TokenResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            RefreshTokenExpiresAt: refreshTokenExpiry,
            AccessTokenExpiresAt: accessTokenExpiry);

        return Task.FromResult(response);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Base64UrlEncoder.Encode(randomNumber);
    }
}