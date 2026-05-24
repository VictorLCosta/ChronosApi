using System.ComponentModel;

namespace Infrastructure.Web.Idempotency;

[ImmutableObject(true)]
public sealed record CachedIdempotentResponse
{
    public int StatusCode { get; init; }
    public string? ContentType { get; init; }
    public byte[] Body { get; init; } = [];
}