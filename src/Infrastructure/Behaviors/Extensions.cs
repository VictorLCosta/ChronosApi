using System.Reflection;

namespace Infrastructure.Behaviors;

public static class Extensions
{
    public static IServiceCollection AddBehaviors(this IServiceCollection services)
    {
        services.AddMediator((options) =>
            {
                options.Assemblies = [Assembly.GetExecutingAssembly()];
                options.PipelineBehaviors = [typeof(CachingBehavior<,>)];
            }
        );

        return services;
    }
}
