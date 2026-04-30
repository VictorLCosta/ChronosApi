using Application.Features.Goals;
using Application.Common.Extensions;

namespace Application.Features.GoalLogs;

public sealed record GetGoalLogByIdQuery(Guid Id) : IQuery<GoalLogDto?>, ICacheable
{
    public bool BypassCache => true;
    public string CacheKey => $"GetGoalLogByIdQuery:{Id}";
    public int SlidingExpirationInMinutes => 5;
    public int AbsoluteExpirationInMinutes => 5;
};

public class GetGoalLogByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : IQueryHandler<GetGoalLogByIdQuery, GoalLogDto?>
{
    public async ValueTask<Result<GoalLogDto?>> Handle(GetGoalLogByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var goalLog = await context.GoalLogs
            .AsNoTracking()
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(gl => gl.Id == request.Id, cancellationToken);

        if (goalLog is null)
            return Result.NotFound();

        var goalLogDto = new GoalLogDto(goalLog.Id, goalLog.Date, goalLog.Notes, goalLog.Completed, goalLog.GoalId);

        return Result.Success<GoalLogDto?>(goalLogDto);
    }
}
