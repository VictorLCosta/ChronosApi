using Application.Features.TaskItems;

using Ardalis.Result.AspNetCore;

using Mediator;

namespace Api.Endpoints;

internal static class TaskItemEndpoints
{
    public static void MapTaskItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/tasks")
            .WithTags("TaskItems")
            .RequireAuthorization();

        group.MapGet("/", async (IMediator sender, [AsParameters] SearchAllTaskItemsQuery query) =>
        {
            var result = await sender.Send(query);

            return result.ToMinimalApiResult();
        })
        .WithName("GetAllTaskItems");

        group.MapGet("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new GetTaskItemByIdQuery(id));

            return result.ToMinimalApiResult();
        })
        .WithName("GetTaskItemById");

        group.MapPost("/", async (IMediator sender, CreateTaskItemCommand command) =>
        {
            var result = await sender.Send(command);

            return result.ToMinimalApiResult();
        })
        .WithName("CreateTaskItem");

        group.MapDelete("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new DeleteTaskItemCommand(id));

            return result.ToMinimalApiResult();
        })
        .WithName("DeleteTaskItem")
        .Produces(StatusCodes.Status204NoContent);

        group.MapPatch("trash/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new TrashTaskItemCommand(id));

            return result.ToMinimalApiResult();
        })
        .WithName("TrashTaskItem")
        .Produces(StatusCodes.Status204NoContent);

        group.MapPatch("restore/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new RestoreTaskItemCommand(id));

            return result.ToMinimalApiResult();
        })
        .WithName("RestoreTaskItem")
        .Produces(StatusCodes.Status204NoContent);
    }
}