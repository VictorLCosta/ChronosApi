using CrossCutting.ExtensionMethods;

namespace Application.Features.Reminders;

public class SearchAllRemindersQuery : IQuery<PagedResponse<ReminderDto>>, IPagedQuery, ICacheable
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }

    public bool BypassCache => true;
    public string CacheKey => $"SearchAllRemindersQuery:{PageNumber}:{PageSize}:{Sort}";
    public int SlidingExpirationInMinutes => 5;
    public int AbsoluteExpirationInMinutes => 5;
    public IReadOnlyCollection<string> CacheTags => ["Reminders"];
};

public class SearchAllRemindersQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : IQueryHandler<SearchAllRemindersQuery, PagedResponse<ReminderDto>>
{
    public async ValueTask<Result<PagedResponse<ReminderDto>>> Handle(SearchAllRemindersQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = currentUserService.GetRequiredUserId();

        var reminders = await context.Reminders
            .AsNoTracking()
            .WhereCreatedBy(userId)
            .OrderByDescending(r => r.RemindAt)
            .ApplySort(request.Sort)
            .Select(r => new ReminderDto(r.Id, r.RemindAt, r.IsSent, r.OffsetMinutes, r.TaskItemId, r.GoalId))
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(reminders);
    }
}