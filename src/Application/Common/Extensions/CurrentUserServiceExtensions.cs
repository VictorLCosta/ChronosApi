using Application.Common.Exceptions;

namespace Application.Common.Extensions;

public static class CurrentUserServiceExtensions
{
    public static string GetRequiredUserId(this ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(currentUserService);

        var userId = currentUserService.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user could not be resolved.");
        }

        return userId;
    }
}