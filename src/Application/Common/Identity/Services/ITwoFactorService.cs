namespace Application.Common.Identity.Services;

public interface ITwoFactorService
{
    Task<bool> IsEnabledAsync(string userId, CancellationToken cancellationToken);
    Task<TwoFactorSetupInfo?> GetSetupInfoAsync(string userId, CancellationToken cancellationToken);
    Task<Result<TwoFactorEnableResult>> EnableAsync(string userId, string code, CancellationToken cancellationToken);
    Task<Result> VerifyAsync(string userId, string code, CancellationToken cancellationToken);
}

public sealed record TwoFactorSetupInfo(
    string SharedKey,
    string AuthenticatorUri,
    bool IsEnabled
);

public sealed record TwoFactorEnableResult(
    bool IsEnabled,
    IReadOnlyCollection<string> RecoveryCodes
);