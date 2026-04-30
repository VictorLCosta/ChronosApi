using Application.Features.Reminders;

using Ardalis.Result.AspNetCore;

using Mediator;

namespace Api.Endpoints;

public static class ReminderEndpoints
{
    public static void MapReminderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/reminders")
            .WithTags("Reminders")
            .RequireAuthorization();

        group.MapGet("/", async (IMediator sender, [AsParameters] SearchAllRemindersQuery query) =>
        {
            var result = await sender.Send(query);

            return result.ToMinimalApiResult();
        })
        .WithName("GetAllReminders");

        group.MapGet("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new GetReminderByIdQuery(id));

            return result.ToMinimalApiResult();
        })
        .WithName("GetReminderById");

        group.MapPost("/", async (IMediator sender, CreateReminderCommand command) =>
        {
            var result = await sender.Send(command);

            return result.ToMinimalApiResult();
        })
        .WithName("CreateReminder");

        group.MapPut("/{id}", async (Guid id, IMediator sender, UpdateReminderCommand command) =>
        {
            command = command with { Id = id };
            var result = await sender.Send(command);

            return result.ToMinimalApiResult();
        })
        .WithName("UpdateReminder");

        group.MapDelete("/{id}", async (Guid id, IMediator sender) =>
        {
            var result = await sender.Send(new DeleteReminderCommand(id));

            return result.ToMinimalApiResult();
        })
        .WithName("DeleteReminder");
    }
}