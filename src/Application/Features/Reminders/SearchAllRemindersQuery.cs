using Application.Common.Extensions;

namespace Application.Features.Reminders;

public class SearchAllRemindersQuery : IQuery<PagedResponse<ReminderDto>>, IPagedQuery
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }
};

public class SearchAllRemindersQueryHandler(IApplicationDbContext context) : IQueryHandler<SearchAllRemindersQuery, PagedResponse<ReminderDto>>
{
    public async ValueTask<Result<PagedResponse<ReminderDto>>> Handle(SearchAllRemindersQuery request, CancellationToken cancellationToken)
    {
        var reminders = await context.Reminders
            .Select(r => new ReminderDto(r.Id, r.RemindAt, r.IsSent, r.OffsetMinutes, r.TaskItemId, r.GoalId))
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(reminders);
    }
}