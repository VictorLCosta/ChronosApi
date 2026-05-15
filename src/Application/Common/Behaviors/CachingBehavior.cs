using Mediator;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public class CachingBehavior<TMessage, TResponse>(
    ILogger<CachingBehavior<TMessage, TResponse>> logger,
    HybridCache cache
)
    : IPipelineBehavior<TMessage, TResponse> where TMessage : ICacheable, IRequest<TResponse>
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(next);

        TResponse response;
        if (message.BypassCache) return await next(message, cancellationToken);

        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(
                message.AbsoluteExpirationInMinutes > 0
                    ? message.AbsoluteExpirationInMinutes
                    : 60),
            LocalCacheExpiration = TimeSpan.FromMinutes(
                message.SlidingExpirationInMinutes > 0
                    ? message.SlidingExpirationInMinutes
                    : 2)
        };

        response = await cache.GetOrCreateAsync(
            message.CacheKey, 
            async ct =>
            {
                logger.LogInformation("Cache miss for key {CacheKey}", message.CacheKey);
                return await next(message, ct);
            }, 
            options, 
            message.CacheTags, 
            cancellationToken
        );

        return response;
    }
}