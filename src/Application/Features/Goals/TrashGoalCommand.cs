using CrossCutting.ExtensionMethods;

namespace Application.Features.Goals;

public sealed record TrashGoalCommand(Guid Id) : ICommand;

public sealed class TrashGoalCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    TimeProvider timeProvider) : ICommandHandler<TrashGoalCommand>
{
    public async ValueTask<Result> Handle(TrashGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var goal = await context.Goals
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (goal is null)
        {
            return Result.NotFound();
        }

        if (goal.IsTrashed)
        {
            return Result.NoContent();
        }

        goal.IsTrashed = true;
        goal.TrashedOnUtc = timeProvider.GetUtcNow();
        goal.TrashedBy = Guid.Parse(userId);

        context.Goals.Update(goal);

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? Result.NoContent()
            : Result.Error("Failed to move goal to trash.");
    }
}