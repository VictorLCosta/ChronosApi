using Application.Features.Projects;

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

        group.MapGet("/", async (ISender sender, [AsParameters] SearchAllProjectsQuery query) =>
        {
            var result = await sender.Send(query);

            return result.ToMinimalApiResult();
        });

        group.MapPost("/", () =>
        {
            return Results.Ok("bumbum gulosso");
        });

        group.MapPut("/{id}", (int id) =>
        {
            return Results.Ok($"bumbum gulosso {id}");
        });
    }
}