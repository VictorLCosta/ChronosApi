namespace Api;

public static class Extensions
{
    public static void AddApiEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api");
    }
}
