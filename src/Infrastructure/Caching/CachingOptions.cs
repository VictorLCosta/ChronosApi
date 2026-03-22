namespace Infrastructure.Caching;

public sealed class CachingOptions
{
    public string Redis { get; set; } = string.Empty;

    /// <summary>
    /// Enable SSL for Redis connection. If null, uses connection string default.
    /// Set to true when using Aspire or cloud Redis that requires SSL.
    /// </summary>
    public bool? EnableSsl { get; set; }

    /// <summary>Default sliding expiration if caller doesn't specify.</summary>
    public TimeSpan? DefaultSlidingExpiration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>Default absolute expiration (cap).</summary>
    public TimeSpan? DefaultAbsoluteExpiration { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>Optional prefix applied to all keys.</summary>
    public string? KeyPrefix { get; set; } = "chronos_";
}