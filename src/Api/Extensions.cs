namespace Api;

public static class Extensions
{
    public static void AddApiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");
    }
}