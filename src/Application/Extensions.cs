using System.Reflection;

using Application.Common.Behaviors;

using FluentValidation;

using Mediator;

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
                options.PipelineBehaviors = [typeof(CachingBehavior<,>), typeof(ValidationBehavior<,>)];
            }
        )
        .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return builder;
    }
}