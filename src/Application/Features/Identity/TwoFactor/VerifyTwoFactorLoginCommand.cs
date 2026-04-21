using Application.Common.Identity.Models;
using Application.Common.Identity.Services;

namespace Application.Features.Identity.TwoFactor;

public sealed class VerifyTwoFactorLoginCommand : ICommand<TokenResponse>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Code { get; set; } = default!;
}

public sealed class VerifyTwoFactorLoginCommandHandler(
    IIdentityService identityService,
    ITokenService tokenService,
    ITwoFactorService twoFactorService) : ICommandHandler<VerifyTwoFactorLoginCommand, TokenResponse>
{
    public async ValueTask<Result<TokenResponse>> Handle(VerifyTwoFactorLoginCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var identityResult = await identityService.ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);
        if (!identityResult.HasValue)
        {
            return Result.Unauthorized("Invalid credentials.");
        }

        var (subject, claims) = identityResult.Value;

        if (!await twoFactorService.IsEnabledAsync(subject, cancellationToken))
        {
            return Result.Invalid(new ValidationError { ErrorMessage = "Two-factor authentication is not enabled for this account." });
        }

        var verifyResult = await twoFactorService.VerifyAsync(subject, request.Code, cancellationToken);
        if (!verifyResult.IsSuccess)
        {
            return verifyResult;
        }

        var token = await tokenService.GetTokenAsync(subject, claims, cancellationToken);
        await identityService.StoreRefreshTokenAsync(subject, token.RefreshToken, token.RefreshTokenExpiresAt, cancellationToken);

        return Result.Success(token);
    }
}