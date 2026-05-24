using CrossCutting.ExtensionMethods;

namespace Application.Features.Goals;

public sealed record RestoreGoalCommand(Guid Id) : ICommand;

public sealed class RestoreGoalCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : ICommandHandler<RestoreGoalCommand>
{
    public async ValueTask<Result> Handle(RestoreGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var goal = await context.Goals
            .IgnoreQueryFilters()
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (goal is null)
        {
            return Result.NotFound();
        }

        if (!goal.IsTrashed)
        {
            return Result.NoContent();
        }

        goal.IsTrashed = false;
        goal.TrashedOnUtc = null;
        goal.TrashedBy = null;

        context.Goals.Update(goal);

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? Result.NoContent()
            : Result.Error("Failed to restore goal.");
    }
}