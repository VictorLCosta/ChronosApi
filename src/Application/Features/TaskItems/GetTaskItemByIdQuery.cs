namespace Application.Features.TaskItems;

public sealed record GetTaskItemByIdQuery(Guid Id) : IQuery<TaskItemDto?>;

public class GetTaskItemByIdQueryHandler(IApplicationDbContext context) : IQueryHandler<GetTaskItemByIdQuery, TaskItemDto?>
{
    public async ValueTask<Result<TaskItemDto?>> Handle(GetTaskItemByIdQuery request, CancellationToken cancellationToken)
    {
        var taskItem = await context.Tasks
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (taskItem is null)
            return Result.NotFound();

        var taskItemDto = new TaskItemDto(taskItem.Id, taskItem.Title, taskItem.Notes, taskItem.DueDate, taskItem.StartDate, taskItem.GoalId, taskItem.ProjectId, taskItem.ParentTaskId);

        return Result.Success<TaskItemDto?>(taskItemDto);
    }
}