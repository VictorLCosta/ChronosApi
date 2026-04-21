using Application.Common.Identity.Models;
using Application.Common.Identity.Services;

using Microsoft.Extensions.Logging;

namespace Application.Features.Identity.Tokens.RefreshToken;

public record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<RefreshTokenResponse>;

public class RefreshTokenCommandHandler(
    IIdentityService identityService,
    IRequestContext requestContext,
    ITokenService tokenService,
    ILogger<RefreshTokenCommandHandler> logger) : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async ValueTask<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var clientId = requestContext.ClientId;

        var validated = await identityService
            .ValidateRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (!validated.HasValue)
        {
            return Result.Unauthorized("Invalid refresh token.");
        }

        var (subject, claims) = validated.Value;

        var newToken = await tokenService.GetTokenAsync(subject, claims, cancellationToken);

        var refreshTokenHash = Sha256Short(request.RefreshToken);
        await identityService.StoreRefreshTokenAsync(subject, newToken.RefreshToken, newToken.RefreshTokenExpiresAt, cancellationToken);

        return new RefreshTokenResponse(
            Token: newToken.AccessToken,
            RefreshToken: newToken.RefreshToken,
            RefreshTokenExpiryTime: newToken.RefreshTokenExpiresAt);
    }

    private static string Sha256Short(string value)
    {
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash.AsSpan(0, 8));
    }
}