using Infrastructure.Cors;
using Infrastructure.Exceptions;
using Infrastructure.Identity;
using Infrastructure.Logging;
using Infrastructure.Middlewares;
using Infrastructure.OpenTelemetry;
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

        builder.ConfigureOpenTelemetry();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddRateLimit(builder.Configuration);

        builder.Services.AddAppIdentity();

        builder.Services.AddAppCors(builder.Configuration);

        builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = ctx =>
            {
                ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
                ctx.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
                ctx.ProblemDetails.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
            });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.AddOptions<SecurityHeadersOptions>().BindConfiguration(nameof(SecurityHeadersOptions));

        return builder;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAppCors();
        app.UseRateLimit();
        app.UseExceptionHandler();

        app.UseMiddlewares(); // custom middlewares
        return app;
    }
}