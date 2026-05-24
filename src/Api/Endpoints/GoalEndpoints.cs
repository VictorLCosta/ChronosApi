using Application.Features.Goals;

using Ardalis.Result.AspNetCore;

using Mediator;

namespace Api.Endpoints;

internal static class GoalEndpoints
{
    public static void MapGoalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/goals")
            .WithTags("Goals")
            .RequireAuthorization();

        group.MapGet("/", async (IMediator sender, [AsParameters] SearchAllGoalsQuery query) =>
        {
            var result = await sender.Send(query);

            return result.ToMinimalApiResult();
        })
        .WithName("GetAllGoals");

        group.MapGet("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new GetGoalByIdQuery(id));

            return result.ToMinimalApiResult();
        })
        .WithName("GetGoalById");

        group.MapPost("/", async (IMediator sender, CreateGoalCommand command) =>
        {
            var result = await sender.Send(command);

            return result.ToMinimalApiResult();
        })
        .WithName("CreateGoal");

        group.MapDelete("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new DeleteGoalCommand(id));

            return result.ToMinimalApiResult();
        })
        .WithName("DeleteGoal")
        .Produces(StatusCodes.Status204NoContent);

        group.MapPatch("trash/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new TrashGoalCommand(id));

            return result.ToMinimalApiResult();
        })
        .WithName("TrashGoal")
        .Produces(StatusCodes.Status204NoContent);

        group.MapPatch("restore/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new RestoreGoalCommand(id));

            return result.ToMinimalApiResult();
        })
        .WithName("RestoreGoal")
        .Produces(StatusCodes.Status204NoContent);
    }
}