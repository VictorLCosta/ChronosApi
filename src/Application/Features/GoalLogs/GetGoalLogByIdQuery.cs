namespace Application.Features.GoalLogs;

public sealed record GetGoalLogByIdQuery(Guid Id) : IQuery<GoalLogDto?>;

public class GetGoalLogByIdQueryHandler(IApplicationDbContext context) : IQueryHandler<GetGoalLogByIdQuery, GoalLogDto?>
{
    public async ValueTask<Result<GoalLogDto?>> Handle(GetGoalLogByIdQuery request, CancellationToken cancellationToken)
    {
        var goalLog = await context.GoalLogs
            .FirstOrDefaultAsync(gl => gl.Id == request.Id, cancellationToken);

        if (goalLog is null)
            return Result.NotFound();

        var goalLogDto = new GoalLogDto(goalLog.Id, goalLog.Date, goalLog.Notes, goalLog.Completed, goalLog.GoalId);

        return Result.Success(goalLogDto);
    }
}