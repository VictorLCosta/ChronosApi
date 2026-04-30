using Application.Common.Extensions;

namespace Application.Features.Reminders;

public sealed record GetReminderByIdQuery(Guid Id) : IQuery<ReminderDto?>, ICacheable
{
    public bool BypassCache => true;
    public string CacheKey => $"GetReminderByIdQuery:{Id}";
    public int SlidingExpirationInMinutes => 5;
    public int AbsoluteExpirationInMinutes => 5;
};

public class GetReminderByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : IQueryHandler<GetReminderByIdQuery, ReminderDto?>
{
    public async ValueTask<Result<ReminderDto?>> Handle(GetReminderByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var reminder = await context.Reminders
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (reminder is null)
            return Result.NotFound();

        var reminderDto = new ReminderDto(reminder.Id, reminder.RemindAt, reminder.IsSent, reminder.OffsetMinutes, reminder.TaskItemId, reminder.GoalId);

        return Result.Success<ReminderDto?>(reminderDto);
    }
}
