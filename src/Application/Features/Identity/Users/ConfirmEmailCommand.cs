using Application.Common.Identity.Services;

namespace Application.Features.Identity.Users;

public sealed record ConfirmEmailCommand(string UserId, string Code) : ICommand<string>;

public class ConfirmEmailCommandHandler(IUserService userService) : ICommandHandler<ConfirmEmailCommand, string>
{
    public async ValueTask<Result<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await userService.ConfirmEmailAsync(request.UserId, request.Code, cancellationToken);

        return result;
    }
}