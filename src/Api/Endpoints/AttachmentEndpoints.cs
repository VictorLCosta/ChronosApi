using Application.Features.Attachments;

using Ardalis.Result.AspNetCore;

using Mediator;

namespace Api.Endpoints;

public static class AttachmentEndpoints
{
    public static void MapAttachmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/attachments")
            .WithTags("Attachments")
            .RequireAuthorization();

        group.MapGet("/", async (IMediator sender, [AsParameters] SearchAllAttachmentsQuery query) =>
        {
            var result = await sender.Send(query);

            return result.ToMinimalApiResult();
        })
        .WithName("GetAllAttachments");

        group.MapGet("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new GetAttachmentByIdQuery(id));

            return result.ToMinimalApiResult();
        })
        .WithName("GetAttachmentById");

        group.MapPost("/", async (IMediator sender, CreateAttachmentCommand command) =>
        {
            var result = await sender.Send(command);

            return result.ToMinimalApiResult();
        })
        .WithName("CreateAttachment");

        group.MapPut("/{id}", async (Guid id, IMediator sender, UpdateAttachmentCommand command) =>
        {
            command = command with { Id = id };
            var result = await sender.Send(command);

            return result.ToMinimalApiResult();
        })
        .WithName("UpdateAttachment");

        group.MapDelete("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new DeleteAttachmentCommand(id));

            return result.ToMinimalApiResult();
        })
        .WithName("DeleteAttachment");
    }
}