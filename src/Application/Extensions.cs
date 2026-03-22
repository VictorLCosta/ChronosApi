using Application.Common.Behaviors;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

public static class Extensions
{
    public static IHostApplicationBuilder AddApplication(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddMediator((options) =>
            {
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.Assemblies = [typeof(Extensions).Assembly];
                options.PipelineBehaviors = [typeof(CachingBehavior<,>)];
            }
        );

        return builder;
    }
}