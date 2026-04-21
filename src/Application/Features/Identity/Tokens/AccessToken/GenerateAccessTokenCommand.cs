using Application.Common.Identity.Models;
using Application.Common.Identity.Services;

using Microsoft.Extensions.Logging;

namespace Application.Features.Identity.Tokens.AccessToken;

public record GenerateAccessTokenCommand(string Email, string Password) : ICommand<TokenResponse>;

public class GenerateAccessTokenCommandHandler(
    IIdentityService identityService,
    ITokenService tokenService,
    ITwoFactorService twoFactorService,
    IRequestContext requestContext,
    ILogger<GenerateAccessTokenCommandHandler> logger
) : ICommandHandler<GenerateAccessTokenCommand, TokenResponse>
{
    public async ValueTask<Result<TokenResponse>> Handle(GenerateAccessTokenCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var ip = requestContext.IpAddress ?? "unknown";
        var ua = requestContext.UserAgent ?? "unknown";
        var clientId = requestContext.ClientId;

        var identityResult = await identityService
            .ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);

        if (!identityResult.HasValue)
        {
            return Result.Unauthorized("Invalid credentials.");
        }

        var (subject, claims) = identityResult.Value;

        if (await twoFactorService.IsEnabledAsync(subject, cancellationToken))
        {
            logger.LogInformation("Password login blocked for user {Subject} because TOTP is enabled.", subject);
            return Result.Unauthorized("Two-factor authentication required. Use /identity/2fa/verify-login.");
        }

        var token = await tokenService.GetTokenAsync(subject, claims, cancellationToken);

        await identityService.StoreRefreshTokenAsync(subject, token.RefreshToken, token.RefreshTokenExpiresAt, cancellationToken);

        return new TokenResponse(
            AccessToken: token.AccessToken,
            RefreshToken: token.RefreshToken,
            AccessTokenExpiresAt: token.AccessTokenExpiresAt,
            RefreshTokenExpiresAt: token.RefreshTokenExpiresAt);
    }
}