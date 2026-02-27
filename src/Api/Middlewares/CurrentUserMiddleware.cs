using Application.Common.Interfaces;

namespace Api.Middlewares;

public class CurrentUserMiddleware(ICurrentUserService currentUserService) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        currentUserService.SetCurrentUser(context.User);
        await next(context);
    }
}
