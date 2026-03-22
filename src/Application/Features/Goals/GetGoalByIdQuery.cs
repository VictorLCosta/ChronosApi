namespace Application.Features.Goals;

public sealed record GetGoalByIdQuery(Guid Id) : IQuery<GoalDto?>;

public class GetGoalByIdQueryHandler(IApplicationDbContext context) : IQueryHandler<GetGoalByIdQuery, GoalDto?>
{
    public async ValueTask<Result<GoalDto?>> Handle(GetGoalByIdQuery request, CancellationToken cancellationToken)
    {
        var goal = await context.Goals
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (goal is null)
            return Result.NotFound();

        var goalDto = new GoalDto(goal.Id, goal.Title, goal.Notes, goal.Status, goal.Priority, goal.ProjectId);

        return Result.Success(goalDto);
    }
}