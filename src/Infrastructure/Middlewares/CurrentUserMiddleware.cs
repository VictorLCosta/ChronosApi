using Application.Common.Interfaces;

using Microsoft.AspNetCore.Http;

namespace Infrastructure.Middlewares;

public class CurrentUserMiddleware(ICurrentUserService currentUserService) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        currentUserService.SetCurrentUser(context.User);
        await next(context);
    }
}