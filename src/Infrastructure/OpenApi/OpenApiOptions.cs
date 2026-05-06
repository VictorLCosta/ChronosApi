namespace Infrastructure.OpenApi;

public class OpenApiOptions
{
    public required string Title { get; init; }
    public required string Description { get; init; }

    internal ContactOptions? Contact { get; init; }
    internal LicenseOptions? License { get; init; }

    internal sealed class ContactOptions
    {
        public string? Name { get; init; }
        public Uri? Url { get; init; }
        public string? Email { get; init; }
    }

    internal sealed class LicenseOptions
    {
        public string? Name { get; init; }
        public Uri? Url { get; init; }
    }
}