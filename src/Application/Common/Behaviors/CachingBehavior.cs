using System.Text;
using System.Text.Json;

using Mediator;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public class CachingBehavior<TMessage, TResponse>(
    ILogger<CachingBehavior<TMessage, TResponse>> logger,
    ICacheService cache
)
    : IPipelineBehavior<TMessage, TResponse> where TMessage : ICacheable, IRequest<TResponse>
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response;
        if (message.BypassCache) return await next(message, cancellationToken);
        async Task<TResponse> GetResponseAndAddToCache()
        {
            response = await next(message, cancellationToken);
            if (response != null)
            {
                var slidingExpiration = message.SlidingExpirationInMinutes == 0 ? 30 : message.SlidingExpirationInMinutes;
                var absoluteExpiration = message.AbsoluteExpirationInMinutes == 0 ? 60 : message.AbsoluteExpirationInMinutes;
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(slidingExpiration))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(absoluteExpiration));

                var serializedData = Encoding.Default.GetBytes(JsonSerializer.Serialize(response));
                await cache.SetItemAsync(message.CacheKey, serializedData, TimeSpan.FromMinutes(absoluteExpiration), cancellationToken);
            }
            return response;
        }
        var cachedResponse = await cache.GetItemAsync<byte[]>(message.CacheKey, cancellationToken);
        if (cachedResponse != null)
        {
            response = JsonSerializer.Deserialize<TResponse>(Encoding.Default.GetString(cachedResponse))!;
            logger.LogInformation("fetched from cache with key : {CacheKey}", message.CacheKey);

            await cache.RefreshItemAsync(message.CacheKey, cancellationToken);
        }
        else
        {
            response = await GetResponseAndAddToCache();
            logger.LogInformation("added to cache with key : {CacheKey}", message.CacheKey);
        }

        return response;
    }
}