using Application.Common.Extensions;

namespace Application.Features.Reminders;

public sealed record ReminderDto(Guid Id, DateTime RemindAt, bool IsSent, int? OffsetMinutes, Guid? TaskItemId, Guid? GoalId);

public sealed record CreateReminderResultDto(Guid Id);

public sealed record CreateReminderCommand(
    DateTime RemindAt,
    int? OffsetMinutes = null,
    Guid? TaskItemId = null,
    Guid? GoalId = null
) : ICommand<CreateReminderResultDto>;

public class CreateReminderCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<CreateReminderCommand, CreateReminderResultDto>
{
    public async ValueTask<Result<CreateReminderResultDto>> Handle(CreateReminderCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var reminder = await context.Reminders.AddAsync(new Domain.Entities.Reminder
        {
            RemindAt = request.RemindAt,
            OffsetMinutes = request.OffsetMinutes,
            TaskItemId = request.TaskItemId,
            GoalId = request.GoalId,
            CreatedBy = userId
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result<CreateReminderResultDto>.Created(new CreateReminderResultDto(reminder.Entity.Id));
    }
}
