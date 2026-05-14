using Infrastructure.Persistence;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.Web.HealthChecks;

public static class Extensions
{
    public static IServiceCollection ConfigureHealthChecks(this IServiceCollection builder)
    {
        builder.AddHealthChecks()
               .AddCheck(
                    name: "self",
                    check: () => HealthCheckResult.Healthy("API process is alive."),
                    tags: ["live"])
               .AddDbContextCheck<ApplicationDbContext>(
                    name: "postgres",
                    tags: ["ready", "db", "critical"])
               .AddCheck<RedisHealthCheck>(
                    name: "redis",
                    failureStatus: HealthStatus.Degraded,
                    tags: ["ready", "cache"],
                    timeout: TimeSpan.FromSeconds(3));

        return builder;
    }
}
