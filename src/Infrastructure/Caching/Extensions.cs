using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;

using StackExchange.Redis;

namespace Infrastructure.Caching;

public static class Extensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<CachingOptions>()
            .BindConfiguration(nameof(CachingOptions));

        var cacheOptions = configuration.GetSection(nameof(CachingOptions)).Get<CachingOptions>() ?? new CachingOptions();
        
        services.AddDistributedMemoryCache();
        
        if (!string.IsNullOrEmpty(cacheOptions.Redis))
        {
            services.AddStackExchangeRedisCache(opt =>
            {
                var config = ConfigurationOptions.Parse(cacheOptions.Redis);
                config.AbortOnConnectFail = false;

                if (cacheOptions.EnableSsl.HasValue)
                {
                    config.Ssl = cacheOptions.EnableSsl.Value;
                }

                opt.ConfigurationOptions = config;
            });
        }

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = cacheOptions.DefaultExpiration,            // L1 + L2 total lifetime
                LocalCacheExpiration = cacheOptions.DefaultLocalCacheExpiration, // L1 only
            };

            options.MaximumKeyLength = cacheOptions.MaximumKeyLength;
            options.MaximumPayloadBytes = cacheOptions.MaximumPayloadBytes;
        });

        return services;
    }
}