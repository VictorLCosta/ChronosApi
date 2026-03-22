using Application.Features.Goals;
using Application.Features.Projects;
using Application.Features.TaskItems;

using Ardalis.Result.AspNetCore;

using Mediator;

namespace Api.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/projects")
            .WithTags("Projects");

        group.MapGet("/", async (IMediator sender, [AsParameters] SearchAllProjectsQuery query) =>
        {
            var result = await sender.Send(query);

            return result.ToMinimalApiResult();
        })
        .WithName("GetAllProjects");

        group.MapGet("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new GetProjectByIdQuery(id));

            return result.ToMinimalApiResult();
        })
        .WithName("GetProjectById");

        group.MapPost("/", async (IMediator sender, CreateProjectCommand command) =>
        {
            var result = await sender.Send(command);

            return result.ToMinimalApiResult();
        })
        .WithName("CreateProject");

        group.MapPut("/{id}", async (Guid id, IMediator sender, UpdateProjectCommand command) =>
        {
            command = command with { Id = id };
            var result = await sender.Send(command);

            return result.ToMinimalApiResult();
        })
        .WithName("UpdateProject");

        group.MapDelete("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new DeleteProjectCommand(id));

            return result.ToMinimalApiResult();
        })
        .WithName("DeleteProject");

        group.MapGet("/{id}/goals", async (Guid id, IMediator sender, [AsParameters] SearchAllGoalsQuery query) =>
        {
            var result = await sender.Send(query);

            return result.ToMinimalApiResult();
        })
        .WithName("GetGoalsByProject");

        group.MapGet("/{id}/tasks", async (Guid id, IMediator sender, [AsParameters] SearchAllTaskItemsQuery query) =>
        {
            var result = await sender.Send(query);

            return result.ToMinimalApiResult();
        })
        .WithName("GetTasksByProject");
    }
}