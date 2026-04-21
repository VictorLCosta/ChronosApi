using Infrastructure.SecurityHeaders;

using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Middlewares;

internal static class Extensions
{
    internal static IServiceCollection AddMiddlewares(this IServiceCollection services) =>
        services
            .AddScoped<CurrentUserMiddleware>()
            .AddScoped<SecurityHeadersMiddleware>();

    internal static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app) =>
        app
            .UseMiddleware<CurrentUserMiddleware>();
}