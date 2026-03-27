namespace Application.Features.Reminders;

public sealed record GetReminderByIdQuery(Guid Id) : IQuery<ReminderDto?>, ICacheable
{
    public bool BypassCache => false;
    public string CacheKey => $"GetReminderByIdQuery:{Id}";
    public int SlidingExpirationInMinutes => 5;
    public int AbsoluteExpirationInMinutes => 5;
};

public class GetReminderByIdQueryHandler(IApplicationDbContext context) : IQueryHandler<GetReminderByIdQuery, ReminderDto?>
{
    public async ValueTask<Result<ReminderDto?>> Handle(GetReminderByIdQuery request, CancellationToken cancellationToken)
    {
        var reminder = await context.Reminders
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (reminder is null)
            return Result.NotFound();

        var reminderDto = new ReminderDto(reminder.Id, reminder.RemindAt, reminder.IsSent, reminder.OffsetMinutes, reminder.TaskItemId, reminder.GoalId);

        return Result.Success<ReminderDto?>(reminderDto);
    }
}