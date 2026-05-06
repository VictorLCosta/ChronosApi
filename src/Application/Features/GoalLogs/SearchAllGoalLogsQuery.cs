using Application.Common.Extensions;

namespace Application.Features.GoalLogs;

public class SearchAllGoalLogsQuery : IQuery<PagedResponse<GoalLogDto>>, IPagedQuery, ICacheable
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }

    public bool BypassCache => true;
    public string CacheKey => $"SearchAllGoalLogsQuery:{PageNumber}:{PageSize}:{Sort}";
    public int SlidingExpirationInMinutes => 5;
    public int AbsoluteExpirationInMinutes => 5;
};

public class SearchAllGoalLogsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : IQueryHandler<SearchAllGoalLogsQuery, PagedResponse<GoalLogDto>>
{
    public async ValueTask<Result<PagedResponse<GoalLogDto>>> Handle(SearchAllGoalLogsQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = currentUserService.GetRequiredUserId();

        var goalLogs = await context.GoalLogs
            .AsNoTracking()
            .WhereCreatedBy(userId)
            .OrderByDescending(gl => gl.Date)
            .Select(gl => new GoalLogDto(gl.Id, gl.Date, gl.Notes, gl.Completed, gl.GoalId))
            .ApplySort(request.Sort)
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(goalLogs);
    }
}