using Application.Common.Extensions;

namespace Application.Features.GoalLogs;

public class SearchAllGoalLogsQuery : IQuery<PagedResponse<GoalLogDto>>, IPagedQuery, ICacheable
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }

    public bool BypassCache => false;
    public string CacheKey => $"SearchAllGoalLogsQuery:{PageNumber}:{PageSize}:{Sort}";
    public int SlidingExpirationInMinutes => 5;
    public int AbsoluteExpirationInMinutes => 5;
};

public class SearchAllGoalLogsQueryHandler(IApplicationDbContext context) : IQueryHandler<SearchAllGoalLogsQuery, PagedResponse<GoalLogDto>>
{
    public async ValueTask<Result<PagedResponse<GoalLogDto>>> Handle(SearchAllGoalLogsQuery request, CancellationToken cancellationToken)
    {
        var goalLogs = await context.GoalLogs
            .Select(gl => new GoalLogDto(gl.Id, gl.Date, gl.Notes, gl.Completed, gl.GoalId))
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(goalLogs);
    }
}