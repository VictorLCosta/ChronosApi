using CrossCutting.ExtensionMethods;

namespace Application.Features.TaskItems;

public sealed record TrashTaskItemCommand(Guid Id) : ICommand;

public sealed class TrashTaskItemCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    TimeProvider timeProvider) : ICommandHandler<TrashTaskItemCommand>
{
    public async ValueTask<Result> Handle(TrashTaskItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var taskItem = await context.Tasks
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (taskItem is null)
        {
            return Result.NotFound();
        }

        if (taskItem.IsTrashed)
        {
            return Result.NoContent();
        }

        taskItem.IsTrashed = true;
        taskItem.TrashedOnUtc = timeProvider.GetUtcNow();
        taskItem.TrashedBy = Guid.Parse(userId);

        context.Tasks.Update(taskItem);

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? Result.NoContent()
            : Result.Error("Failed to move task item to trash.");
    }
}