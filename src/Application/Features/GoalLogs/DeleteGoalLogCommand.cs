namespace Application.Features.GoalLogs;

public sealed record DeleteGoalLogCommand(Guid Id) : ICommand<Unit>;

public class DeleteGoalLogCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteGoalLogCommand, Unit>
{
    public async ValueTask<Result<Unit>> Handle(DeleteGoalLogCommand request, CancellationToken cancellationToken)
    {
        var goalLog = await context.GoalLogs.FirstOrDefaultAsync(gl => gl.Id == request.Id, cancellationToken);

        if (goalLog is null)
            return Result.NotFound();

        context.GoalLogs.Remove(goalLog);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}