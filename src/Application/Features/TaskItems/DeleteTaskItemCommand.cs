using Application.Common.Extensions;

namespace Application.Features.TaskItems;

public sealed record DeleteTaskItemCommand(Guid Id) : ICommand<Unit>;

public class DeleteTaskItemCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<DeleteTaskItemCommand, Unit>
{
    public async ValueTask<Result<Unit>> Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var taskItem = await context.Tasks
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (taskItem is null)
            return Result.NotFound();

        context.Tasks.Remove(taskItem);
        await context.SaveChangesAsync(cancellationToken);

        return Result.NoContent();
    }
}
