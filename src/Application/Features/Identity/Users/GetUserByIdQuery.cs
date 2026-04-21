using Application.Common.Identity.Models;
using Application.Common.Identity.Services;

namespace Application.Features.Identity.Users;

public sealed record GetUserByIdQuery(string UserId) : IQuery<UserDto>;

public class GetUserByIdQueryHandler(IUserService userService) : IQueryHandler<GetUserByIdQuery, UserDto>
{
    public async ValueTask<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await userService.GetAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result.NotFound("User not found");
        }

        return Result.Success(user);

    }
}