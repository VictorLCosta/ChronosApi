using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.Web.HealthChecks;

public class RedisHealthCheck(IDistributedCache cache) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            const string key = "__health_check__";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
            };
            await cache.SetStringAsync(key, "ok", options, cancellationToken).ConfigureAwait(false);
            await cache.RemoveAsync(key, cancellationToken).ConfigureAwait(false);

            return HealthCheckResult.Healthy(
                "Redis is accessible.",
                data: new Dictionary<string, object>
                {
                    ["category"] = "cache",
                    ["probe"] = "write-delete"
                });
        }
        catch (Exception ex)
        {

            return new HealthCheckResult(
                context.Registration.FailureStatus,
                "Redis is not accessible.",
                ex,
                new Dictionary<string, object>
                {
                    ["category"] = "cache",
                    ["probe"] = "write-delete"
                });
        }
    }
}