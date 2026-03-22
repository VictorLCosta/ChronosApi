namespace Application.Features.Goals;

public sealed record DeleteGoalCommand(Guid Id) : ICommand<Unit>;

public class DeleteGoalCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteGoalCommand, Unit>
{
    public async ValueTask<Result<Unit>> Handle(DeleteGoalCommand request, CancellationToken cancellationToken)
    {
        var goal = await context.Goals.FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (goal is null)
            return Result.NotFound();

        context.Goals.Remove(goal);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}