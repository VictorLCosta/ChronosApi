using Application.Common.Interfaces;

using Microsoft.Extensions.Configuration;

using StackExchange.Redis;

namespace Infrastructure.Caching;

public static class Extensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<CachingOptions>()
            .BindConfiguration(nameof(CachingOptions));

        services.AddMemoryCache();

        var cacheOptions = configuration.GetSection(nameof(CachingOptions)).Get<CachingOptions>();
        if (cacheOptions == null || string.IsNullOrEmpty(cacheOptions.Redis))
        {
            // If no Redis, use memory cache for L2 as well
            services.AddDistributedMemoryCache();
            services.AddTransient<ICacheService, HybridCacheService>();
            return services;
        }

        services.AddStackExchangeRedisCache(options =>
        {
            var config = ConfigurationOptions.Parse(cacheOptions.Redis);
            config.AbortOnConnectFail = true;

            // Only override SSL if explicitly configured
            if (cacheOptions.EnableSsl.HasValue)
            {
                config.Ssl = cacheOptions.EnableSsl.Value;
            }

            options.ConfigurationOptions = config;
        });

        // Register hybrid cache service
        services.AddTransient<ICacheService, HybridCacheService>();

        return services;
    }
}