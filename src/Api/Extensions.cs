using Api.Endpoints;

namespace Api;

public static class Extensions
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");

        group.MapProjectEndpoints();
    }
}