using Application.Features.GoalLogs;

using Ardalis.Result.AspNetCore;

using Mediator;

namespace Api.Endpoints;

public static class GoalLogEndpoints
{
    public static void MapGoalLogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/goallogs")
            .WithTags("GoalLogs");

        group.MapGet("/", async (IMediator sender, [AsParameters] SearchAllGoalLogsQuery query) =>
        {
            var result = await sender.Send(query);

            return result.ToMinimalApiResult();
        })
        .WithName("GetAllGoalLogs");

        group.MapGet("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new GetGoalLogByIdQuery(id));

            return result.ToMinimalApiResult();
        })
        .WithName("GetGoalLogById");

        group.MapPost("/", async (IMediator sender, CreateGoalLogCommand command) =>
        {
            var result = await sender.Send(command);

            return result.ToMinimalApiResult();
        })
        .WithName("CreateGoalLog");

        group.MapPut("/{id}", async (Guid id, IMediator sender, UpdateGoalLogCommand command) =>
        {
            command = command with { Id = id };
            var result = await sender.Send(command);

            return result.ToMinimalApiResult();
        })
        .WithName("UpdateGoalLog");

        group.MapDelete("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new DeleteGoalLogCommand(id));

            return result.ToMinimalApiResult();
        })
        .WithName("DeleteGoalLog");
    }
}