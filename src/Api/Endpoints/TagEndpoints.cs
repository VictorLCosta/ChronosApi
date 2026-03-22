using Application.Features.Tags;

using Ardalis.Result.AspNetCore;

using Mediator;

namespace Api.Endpoints;

public static class TagEndpoints
{
    public static void MapTagEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/tags")
            .WithTags("Tags");

        group.MapGet("/", async (IMediator sender, [AsParameters] SearchAllTagsQuery query) =>
        {
            var result = await sender.Send(query);

            return result.ToMinimalApiResult();
        })
        .WithName("GetAllTags");

        group.MapGet("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new GetTagByIdQuery(id));

            return result.ToMinimalApiResult();
        })
        .WithName("GetTagById");

        group.MapPost("/", async (IMediator sender, CreateTagCommand command) =>
        {
            var result = await sender.Send(command);

            return result.ToMinimalApiResult();
        })
        .WithName("CreateTag");

        group.MapPut("/{id}", async (Guid id, IMediator sender, UpdateTagCommand command) =>
        {
            command = command with { Id = id };
            var result = await sender.Send(command);

            return result.ToMinimalApiResult();
        })
        .WithName("UpdateTag");

        group.MapDelete("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new DeleteTagCommand(id));

            return result.ToMinimalApiResult();
        })
        .WithName("DeleteTag");
    }
}