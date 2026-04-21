using Application.Common.Identity.Services;
using Application.Common.Interfaces;

namespace Application.Features.Identity.TwoFactor;

public sealed record TwoFactorSetupResponse(
    string SharedKey,
    string AuthenticatorUri,
    bool IsEnabled
);

public sealed class GetTwoFactorSetupCommand : ICommand<TwoFactorSetupResponse>
{
}

public sealed class GetTwoFactorSetupCommandHandler(
    ICurrentUserService currentUserService,
    ITwoFactorService twoFactorService) : ICommandHandler<GetTwoFactorSetupCommand, TwoFactorSetupResponse>
{
    public async ValueTask<Result<TwoFactorSetupResponse>> Handle(GetTwoFactorSetupCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Unauthorized();
        }

        var setup = await twoFactorService.GetSetupInfoAsync(userId, cancellationToken);
        if (setup is null)
        {
            return Result.NotFound();
        }

        return Result.Success(new TwoFactorSetupResponse(
            setup.SharedKey,
            setup.AuthenticatorUri,
            setup.IsEnabled));
    }
}