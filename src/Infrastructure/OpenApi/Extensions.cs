using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

using Scalar.AspNetCore;

namespace Infrastructure.OpenApi;

public static class Extensions
{
    public static IServiceCollection AddAppOpenApi(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<OpenApiOptions>()
            .Bind(configuration.GetSection(nameof(OpenApiOptions)))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Title), "OpenApi:Title is required.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Description), "OpenApi:Description is required.")
            .ValidateOnStart();

        services.AddEndpointsApiExplorer();
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddDocumentTransformer((document, context, _) =>
                {
                    var provider = context.ApplicationServices;
                    var openApi = provider.GetRequiredService<IOptions<OpenApiOptions>>().Value;

                    document.Info = new OpenApiInfo
                    {
                        Title = openApi.Title,
                        Description = openApi.Description,
                        Contact = openApi.Contact is null ? null : new OpenApiContact
                        {
                            Name = openApi.Contact.Name,
                            Url = openApi.Contact.Url,
                            Email = openApi.Contact.Email
                        },
                        License = openApi.License is null ? null : new OpenApiLicense
                        {
                            Name = openApi.License.Name,
                            Url = openApi.License.Url
                        }
                    };
                    return Task.CompletedTask;
                });
        });

        return services;
    }

    public static WebApplication UseAppOpenApi(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapOpenApi();

        app.MapScalarApiReference("api-docs", options =>
        {
            var configuration = app.Configuration;

            options
                .WithTitle(configuration["OpenApi:Title"] ?? "Chronos API")
                .EnableDarkMode()
                .HideModels()
                .AddPreferredSecuritySchemes("Bearer");
        });

        return app;
    }
}
