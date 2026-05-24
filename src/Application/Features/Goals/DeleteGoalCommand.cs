using CrossCutting.ExtensionMethods;

namespace Application.Features.Goals;

public sealed record DeleteGoalCommand(Guid Id) : ICommand<Unit>;

public class DeleteGoalCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    TimeProvider timeProvider) : ICommandHandler<DeleteGoalCommand, Unit>
{
    public async ValueTask<Result<Unit>> Handle(DeleteGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var goal = await context.Goals
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (goal is null)
            return Result.NotFound();

        if (!goal.IsTrashed)
        {
            goal.IsTrashed = true;
            goal.TrashedOnUtc = timeProvider.GetUtcNow();
            goal.TrashedBy = Guid.Parse(userId);
            context.Goals.Update(goal);
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.NoContent();
    }
}