using Api.Endpoints;

namespace Api;

public static class Extensions
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");

        group.MapProjectEndpoints();
        group.MapGoalEndpoints();
        group.MapIdentityEndpoints();
        group.MapTaskItemEndpoints();
        group.MapReminderEndpoints();
        group.MapTagEndpoints();
        group.MapAttachmentEndpoints();
        group.MapGoalLogEndpoints();
    }
}