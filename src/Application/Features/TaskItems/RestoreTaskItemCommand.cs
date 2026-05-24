using CrossCutting.ExtensionMethods;

namespace Application.Features.TaskItems;

public sealed record RestoreTaskItemCommand(Guid Id) : ICommand;

public sealed class RestoreTaskItemCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : ICommandHandler<RestoreTaskItemCommand>
{
    public async ValueTask<Result> Handle(RestoreTaskItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var taskItem = await context.Tasks
            .IgnoreQueryFilters()
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (taskItem is null)
        {
            return Result.NotFound();
        }

        if (!taskItem.IsTrashed)
        {
            return Result.NoContent();
        }

        taskItem.IsTrashed = false;
        taskItem.TrashedOnUtc = null;
        taskItem.TrashedBy = null;

        context.Tasks.Update(taskItem);

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? Result.NoContent()
            : Result.Error("Failed to restore task item.");
    }
}