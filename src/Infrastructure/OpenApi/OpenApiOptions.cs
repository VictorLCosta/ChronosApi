namespace Infrastructure.OpenApi;

public class OpenApiOptions
{
    public required string Title { get; init; }
    public required string Description { get; init; }

    public ContactOptions? Contact { get; init; }
    public LicenseOptions? License { get; init; }

    public sealed class ContactOptions
    {
        public string? Name { get; init; }
        public Uri? Url { get; init; }
        public string? Email { get; init; }
    }

    public sealed class LicenseOptions
    {
        public string? Name { get; init; }
        public Uri? Url { get; init; }
    }
}
