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
        var userId = currentUserService.GetRequiredUserId();

        var goalLogs = await context.GoalLogs
            .WhereCreatedBy(userId)
            .Select(gl => new GoalLogDto(gl.Id, gl.Date, gl.Notes, gl.Completed, gl.GoalId))
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(goalLogs);
    }
}
