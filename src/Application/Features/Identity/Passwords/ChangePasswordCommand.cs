using System.Text.Json.Serialization;

using Application.Common.Identity.Services;
using Application.Common.Interfaces;

namespace Application.Features.Identity.Passwords;

public class ChangePasswordCommand : ICommand
{
    public string Password { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
    public string ConfirmNewPassword { get; set; } = default!;

    [JsonIgnore]
    public string? UserId { get; set; }
}

public class ChangePasswordCommandHandler(
    ICurrentUserService currentUserService,
    IUserPasswordService userPasswordService) : ICommandHandler<ChangePasswordCommand>
{
    public async ValueTask<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!currentUserService.IsAuthenticated())
        {
            return Result.Unauthorized("User is not authenticated");
        }

        var userId = request.UserId ?? currentUserService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Unauthorized();
        }

        var result = await userPasswordService.ChangePasswordAsync(
            request.Password,
            request.NewPassword,
            request.ConfirmNewPassword,
            userId,
            cancellationToken);

        return result.Status switch
        {
            ResultStatus.Ok => Result.Success(),
            ResultStatus.Invalid => Result.Invalid(result.ValidationErrors),
            ResultStatus.NotFound => Result.NotFound(result.Errors.ToArray()),
            ResultStatus.Unauthorized => Result.Unauthorized(result.Errors.ToArray()),
            _ => Result.Error()
        };
    }

}