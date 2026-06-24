namespace Infrastructure.Web.Idempotency;

public class IdempotencyOptions
{
    public string HeaderName { get; set; } = "Idempotency-Key";

    public TimeSpan DefaultTtl { get; set; } = TimeSpan.FromHours(24);

    public int MaxKeyLength { get; set; } = 128;
}