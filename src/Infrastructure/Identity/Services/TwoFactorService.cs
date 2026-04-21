using System.Globalization;

using Application.Common.Identity.Services;

using Ardalis.Result;

using Infrastructure.Auth;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity.Services;

public sealed class TwoFactorService(
    UserManager<AppUser> userManager,
    IOptions<JwtOptions> jwtOptions) : ITwoFactorService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<bool> IsEnabledAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is not null && user.TwoFactorEnabled;
    }

    public async Task<TwoFactorSetupInfo?> GetSetupInfoAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        var key = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrWhiteSpace(key))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            key = await userManager.GetAuthenticatorKeyAsync(user);
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        var email = user.Email ?? user.UserName ?? user.Id;
        var issuer = string.IsNullOrWhiteSpace(_jwtOptions.Issuer) ? "ChronosApi" : _jwtOptions.Issuer;
        var authenticatorUri = BuildAuthenticatorUri(issuer, email, key);

        return new TwoFactorSetupInfo(
            SharedKey: key,
            AuthenticatorUri: authenticatorUri,
            IsEnabled: user.TwoFactorEnabled);
    }

    public async Task<Result<TwoFactorEnableResult>> EnableAsync(string userId, string code, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null || !user.IsActive)
        {
            return Result.NotFound("User not found.");
        }

        var normalizedCode = NormalizeCode(code);
        if (!await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, normalizedCode))
        {
            return Result.Invalid([new ValidationError { ErrorMessage = "Invalid authenticator code." }]);
        }

        var enableResult = await userManager.SetTwoFactorEnabledAsync(user, true);
        if (!enableResult.Succeeded)
        {
            return Result.Invalid(enableResult.Errors.Select(e => new ValidationError { ErrorMessage = e.Description }).ToList());
        }

        var recoveryCodes = (await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10))?.ToArray();

        if (recoveryCodes is null || recoveryCodes.Length == 0)
        {
            return Result.Error("Failed to generate recovery codes.");
        }

        return Result.Success(new TwoFactorEnableResult(true, recoveryCodes));
    }

    public async Task<Result> VerifyAsync(string userId, string code, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null || !user.IsActive)
        {
            return Result.NotFound("User not found.");
        }

        if (!user.TwoFactorEnabled)
        {
            return Result.Invalid([new ValidationError { ErrorMessage = "Two-factor authentication is not enabled." }]);
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            return Result.Unauthorized("User is locked out.");
        }

        var normalizedCode = NormalizeCode(code);
        var valid = await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, normalizedCode);

        if (!valid)
        {
            await userManager.AccessFailedAsync(user);
            return Result.Unauthorized("Invalid authenticator code.");
        }

        await userManager.ResetAccessFailedCountAsync(user);
        return Result.Success();
    }

    private static string NormalizeCode(string code) =>
        code.Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal);

    private static string BuildAuthenticatorUri(string issuer, string email, string key)
    {
        var encodedIssuer = Uri.EscapeDataString(issuer);
        var encodedEmail = Uri.EscapeDataString(email);

        return string.Format(
            CultureInfo.InvariantCulture,
            "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
            encodedIssuer,
            encodedEmail,
            key);
    }
}