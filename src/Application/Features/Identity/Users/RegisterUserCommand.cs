using System.Text.Json.Serialization;

using Application.Common.Identity.Models;
using Application.Common.Identity.Services;

namespace Application.Features.Identity.Users;

public class RegisterUserCommand : ICommand<RegisterUserResponse>
{
    public string Name { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;

    [JsonIgnore]
    public string Origin { get; set; } = null!;
}

public class RegisterUserCommandHandler(IUserService userService) : ICommandHandler<RegisterUserCommand, RegisterUserResponse>
{
    public async ValueTask<Result<RegisterUserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await userService.CreateAsync(
            request.Name,
            request.UserName,
            request.Email,
            request.Password,
            request.ConfirmPassword,
            "",
            request.Origin,
            cancellationToken
        );

        return Result.Created(new RegisterUserResponse(result));
    }
}