namespace Api.Endpoints;

public static class IdentityEndpoints
{
    public static void MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/identity")
            .WithTags("Identity");

        group.MapPost("/login", async () =>
        {
            
        });

        group.MapPost("/register", async () =>
        {
            
        });

        group.MapPost("/refresh", async () =>
        {
            
        });
    }
}
