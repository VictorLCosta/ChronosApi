using Microsoft.AspNetCore.Identity;

namespace Application.Features.Identity;

public record CreateUserResultDto;

public record RegisterUserCommand(string Email, string Password) : ICommand<CreateUserResultDto>;

public class RegisterUserCommandHandler(UserManager<AppUser> userManager) : ICommandHandler<RegisterUserCommand, CreateUserResultDto>
{
    public async ValueTask<Result<CreateUserResultDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        
    }
}
