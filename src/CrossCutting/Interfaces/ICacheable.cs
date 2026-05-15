namespace CrossCutting.Interfaces;

public interface ICacheable
{
    bool BypassCache { get; }
    string CacheKey { get; }
    int SlidingExpirationInMinutes { get; }
    int AbsoluteExpirationInMinutes { get; }
    IReadOnlyCollection<string> CacheTags { get; }
}