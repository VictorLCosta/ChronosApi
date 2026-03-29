using Application.Common.Extensions;

using Domain.Enums;

namespace Application.Features.Goals;

public sealed record GoalDto(Guid Id, string Title, string? Notes, GoalStatus Status, PriorityLevel Priority, Guid? ProjectId);

public class SearchAllGoalsQuery : IQuery<PagedResponse<GoalDto>>, IPagedQuery, ICacheable
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }
    public string? Q { get; set; }
    public Guid? ProjectId { get; set; }

    public bool BypassCache => false;
    public string CacheKey => $"SearchAllGoalsQuery:{PageNumber}:{PageSize}:{Sort}:{Q}";
    public int SlidingExpirationInMinutes => 5;
    public int AbsoluteExpirationInMinutes => 5;
};

public class SearchAllGoalsQueryHandler(IApplicationDbContext context) : IQueryHandler<SearchAllGoalsQuery, PagedResponse<GoalDto>>
{
    public async ValueTask<Result<PagedResponse<GoalDto>>> Handle(SearchAllGoalsQuery request, CancellationToken cancellationToken)
    {
        var searchQuery = request.Q.NormalizeSearchQuery();

        var goals = await context.Goals
            .WhereIf(request.ProjectId.HasValue, x => x.ProjectId == request.ProjectId)
            .WhereIf(!string.IsNullOrWhiteSpace(searchQuery), x => x.SearchVector.Matches(EF.Functions.PlainToTsQuery(searchQuery)))
            .Select(g => new GoalDto(g.Id, g.Title, g.Notes, g.Status, g.Priority, g.ProjectId))
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(goals);
    }
}