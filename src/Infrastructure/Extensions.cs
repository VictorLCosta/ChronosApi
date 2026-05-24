using Application.Common.Interfaces;

using Infrastructure.Caching;
using Infrastructure.Identity;
using Infrastructure.Logging;
using Infrastructure.OpenApi;
using Infrastructure.OpenTelemetry;
using Infrastructure.Persistence;
using Infrastructure.Web.Authentication;
using Infrastructure.Web.Cors;
using Infrastructure.Web.ExceptionHandlers;
using Infrastructure.Web.HealthChecks;
using Infrastructure.Web.Idempotency;
using Infrastructure.Web.Middlewares;
using Infrastructure.Web.RateLimiting;
using Infrastructure.Web.SecurityHeaders;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class Extensions
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });
        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        builder.Services.AddAppIdentity();

        builder.Services.AddJwtAuth(builder.Configuration);

        builder.Services.AddMiddlewares(); // custom middlewares

        builder.AddAppLogging();

        builder.ConfigureOpenTelemetry();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddRateLimit(builder.Configuration);

        builder.Services.AddAppCors(builder.Configuration);

        builder.Services.AddAppOpenApi(builder.Configuration);

        builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = ctx =>
            {
                ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
                ctx.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
                ctx.ProblemDetails.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
            });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.ConfigureHealthChecks();

        builder.Services.AddCaching(builder.Configuration);

        builder.Services.AddIdempotency(builder.Configuration);

        builder.Services
            .AddOptions<SecurityHeadersOptions>()
            .BindConfiguration(nameof(SecurityHeadersOptions));

        return builder;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseAppCors();
        app.UseHttpsRedirection();
        app.UseResponseCompression();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimit();
        app.UseExceptionHandler();

        app.MapHealthEndpoints();

        app.UseAppOpenApi();

        app.UseMiddlewares(); // custom middlewares
        return app;
    }

    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

        await initializer.MigrateAsync(cancellationToken);
        await initializer.SeedAsync(cancellationToken);
    }
}