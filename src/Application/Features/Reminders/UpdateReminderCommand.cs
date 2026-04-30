using Application.Common.Extensions;

namespace Application.Features.Reminders;

public sealed record UpdateReminderResultDto(Guid Id);

public sealed record UpdateReminderCommand(
    Guid Id,
    DateTime? RemindAt = null,
    bool? IsSent = null,
    int? OffsetMinutes = null,
    Guid? TaskItemId = null,
    Guid? GoalId = null
) : ICommand<UpdateReminderResultDto>;

public class UpdateReminderCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<UpdateReminderCommand, UpdateReminderResultDto>
{
    public async ValueTask<Result<UpdateReminderResultDto>> Handle(UpdateReminderCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var reminder = await context.Reminders
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (reminder is null)
            return Result.NotFound();

        if (request.RemindAt.HasValue) reminder.RemindAt = request.RemindAt.Value;
        if (request.IsSent.HasValue) reminder.IsSent = request.IsSent.Value;
        if (request.OffsetMinutes.HasValue) reminder.OffsetMinutes = request.OffsetMinutes.Value;
        if (request.TaskItemId.HasValue) reminder.TaskItemId = request.TaskItemId.Value;
        if (request.GoalId.HasValue) reminder.GoalId = request.GoalId.Value;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateReminderResultDto(reminder.Id));
    }
}
