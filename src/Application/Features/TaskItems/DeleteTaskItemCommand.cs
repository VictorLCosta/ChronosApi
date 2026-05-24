using CrossCutting.ExtensionMethods;

namespace Application.Features.TaskItems;

public sealed record DeleteTaskItemCommand(Guid Id) : ICommand<Unit>;

public class DeleteTaskItemCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    TimeProvider timeProvider) : ICommandHandler<DeleteTaskItemCommand, Unit>
{
    public async ValueTask<Result<Unit>> Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var taskItem = await context.Tasks
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (taskItem is null)
            return Result.NotFound();

        if (!taskItem.IsTrashed)
        {
            taskItem.IsTrashed = true;
            taskItem.TrashedOnUtc = timeProvider.GetUtcNow();
            taskItem.TrashedBy = Guid.Parse(userId);
            context.Tasks.Update(taskItem);
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.NoContent();
    }
}