using System.Threading.RateLimiting;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Web.RateLimiting;

public static class Extensions
{
    public static IServiceCollection AddRateLimit(this IServiceCollection services, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        services.Configure<RateLimitOptions>(config.GetSection(nameof(RateLimitOptions)));

        var options = config.GetSection(nameof(RateLimitOptions)).Get<RateLimitOptions>();
        if (options is { EnableRateLimiting: true })
        {
            services.AddRateLimiter(rateLimitOptions =>
            {
                rateLimitOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.Request.Headers.Host.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = options.PermitLimit,
                            Window = TimeSpan.FromSeconds(options.WindowInSeconds)
                        });
                });

                rateLimitOptions.RejectionStatusCode = options.RejectionStatusCode;
                rateLimitOptions.OnRejected = async (context, token) =>
                {
                    var message = BuildRateLimitResponseMessage(context);

                    await context.HttpContext.Response.WriteAsync(message, cancellationToken: token);
                };
            });
        }

        return services;
    }

    public static IApplicationBuilder UseRateLimit(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var options = app.ApplicationServices.GetRequiredService<IOptions<RateLimitOptions>>().Value;

        if (options.EnableRateLimiting)
        {
            app.UseRateLimiter();
        }

        return app;
    }


    private static string BuildRateLimitResponseMessage(OnRejectedContext onRejectedContext)
    {
        var hostName = onRejectedContext.HttpContext.Request.Headers.Host.ToString();

        return $"You have reached the maximum number of requests allowed for the address ({hostName}).";
    }
}