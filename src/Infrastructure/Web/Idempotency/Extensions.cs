using Microsoft.Extensions.Configuration;

namespace Infrastructure.Web.Idempotency;

public static class Extensions
{
    public static IServiceCollection AddIdempotency(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<IdempotencyOptions>()
            .BindConfiguration(nameof(IdempotencyOptions));

        return services;
    }
}