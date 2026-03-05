using Infrastructure.Identity;
using Infrastructure.Logging;
using Infrastructure.Middlewares;
using Infrastructure.Persistence;
using Infrastructure.RateLimit;
using Infrastructure.SecurityHeaders;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class Extensions
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddMiddlewares(); // custom middlewares

        builder.AddAppLogging();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddRateLimit(builder.Configuration);

        builder.Services.AddAppIdentity();

        builder.Services.AddProblemDetails();

        builder.Services.AddOptions<SecurityHeadersOptions>().BindConfiguration(nameof(SecurityHeadersOptions));

        return builder;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseRateLimit();

        app.UseMiddlewares(); // custom middlewares
        return app;
    }
}