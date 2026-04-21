using System.Text.Json.Serialization;

using Application.Common.Identity.Services;

namespace Application.Features.Identity.Passwords;

public class ForgotPasswordCommand : ICommand<string>
{
    public string Email { get; set; } = default!;

    [JsonIgnore]
    public string Origin { get; set; } = default!;
}

public class ForgotPasswordCommandHandler(IUserPasswordService userPasswordService) : ICommandHandler<ForgotPasswordCommand, string>
{
    public async ValueTask<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await userPasswordService.ForgotPasswordAsync(request.Email, request.Origin, cancellationToken);

        return result;
    }
}