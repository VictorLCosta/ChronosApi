using Application.Common.Identity.Services;

namespace Application.Features.Identity.Passwords;

public class ResetPasswordCommand : ICommand<string>
{
    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string Token { get; set; } = default!;
}

public class ResetPasswordCommandHandler(IUserPasswordService userPasswordService) : ICommandHandler<ResetPasswordCommand, string>
{
    public async ValueTask<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await userPasswordService.ResetPasswordAsync(request.Email, request.Password, request.Token, cancellationToken);
    }
}