using Application.Common.Identity.Services;

namespace Application.Features.Identity.TwoFactor;

public sealed class VerifyTwoFactorCommand : ICommand
{
    public string Code { get; set; } = default!;
}

public sealed class VerifyTwoFactorCommandHandler(
    ICurrentUserService currentUserService,
    ITwoFactorService twoFactorService) : ICommandHandler<VerifyTwoFactorCommand>
{
    public async ValueTask<Result> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = currentUserService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Unauthorized();
        }

        var result = await twoFactorService.VerifyAsync(userId, request.Code, cancellationToken);

        return result;
    }
}