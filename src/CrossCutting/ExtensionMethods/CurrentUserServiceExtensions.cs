using CrossCutting.Exceptions;

namespace CrossCutting.ExtensionMethods;

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