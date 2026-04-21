using Application.Common.Identity.Services;

namespace Application.Features.Identity.TwoFactor;

public sealed class EnableTwoFactorCommand : ICommand<TwoFactorEnableResult>
{
    public string Code { get; set; } = default!;
}

public sealed class EnableTwoFactorCommandHandler(
    ICurrentUserService currentUserService,
    ITwoFactorService twoFactorService) : ICommandHandler<EnableTwoFactorCommand, TwoFactorEnableResult>
{
    public async ValueTask<Result<TwoFactorEnableResult>> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = currentUserService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Unauthorized();
        }

        var result = await twoFactorService.EnableAsync(userId, request.Code, cancellationToken);

        return result;
    }
}