using Application.Common.Extensions;

namespace Application.Features.Goals;

public sealed record GetGoalByIdQuery(Guid Id) : IQuery<GoalDto?>, ICacheable
{
    public bool BypassCache => true;
    public string CacheKey => $"GetGoalByIdQuery:{Id}";
    public int SlidingExpirationInMinutes => 5;
    public int AbsoluteExpirationInMinutes => 5;
};

public class GetGoalByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : IQueryHandler<GetGoalByIdQuery, GoalDto?>
{
    public async ValueTask<Result<GoalDto?>> Handle(GetGoalByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var goal = await context.Goals
            .AsNoTracking()
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (goal is null)
            return Result.NotFound();

        var goalDto = new GoalDto(goal.Id, goal.Title, goal.Notes, goal.Status, goal.Priority, goal.ProjectId);

        return Result.Success<GoalDto?>(goalDto);
    }
}
