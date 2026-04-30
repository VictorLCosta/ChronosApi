using Application.Common.Extensions;

namespace Application.Features.TaskItems;

public sealed record TaskItemDto(Guid Id, string Title, string? Notes, DateTime? DueDate, DateTime? StartDate, Guid? GoalId, Guid? ProjectId, Guid? ParentTaskId);

public class SearchAllTaskItemsQuery : IQuery<PagedResponse<TaskItemDto>>, IPagedQuery, ICacheable
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }
    public string? Q { get; set; }
    public Guid? ProjectId { get; set; }

    public bool BypassCache => true;
    public string CacheKey => $"SearchAllTaskItemsQuery:{PageNumber}:{PageSize}:{Sort}:{Q}:{ProjectId}";
    public int SlidingExpirationInMinutes => 5;
    public int AbsoluteExpirationInMinutes => 5;
};

public class SearchAllTaskItemsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : IQueryHandler<SearchAllTaskItemsQuery, PagedResponse<TaskItemDto>>
{
    public async ValueTask<Result<PagedResponse<TaskItemDto>>> Handle(SearchAllTaskItemsQuery request, CancellationToken cancellationToken)
    {
        var searchQuery = request.Q.NormalizeSearchQuery();

        var userId = currentUserService.GetRequiredUserId();

        var taskItems = await context.Tasks
            .AsNoTracking()
            .WhereCreatedBy(userId)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Q), x => x.SearchVector.Matches(EF.Functions.PlainToTsQuery(searchQuery)))
            .WhereIf(request.ProjectId != null, x => x.ProjectId == request.ProjectId)
            .Select(t => new TaskItemDto(t.Id, t.Title, t.Notes, t.DueDate, t.StartDate, t.GoalId, t.ProjectId, t.ParentTaskId))
            .ApplySort(request.Sort)
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(taskItems);
    }
}
