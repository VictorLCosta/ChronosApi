namespace Application.Features.TaskItems;

public sealed record UpdateTaskItemResultDto(Guid Id, string Title);

public sealed record UpdateTaskItemCommand(
    Guid Id,
    string? Title = null,
    string? Notes = null,
    DateTime? DueDate = null,
    DateTime? StartDate = null,
    Guid? GoalId = null,
    Guid? ProjectId = null,
    Guid? ParentTaskId = null
) : ICommand<UpdateTaskItemResultDto>;

public class UpdateTaskItemCommandHandler(IApplicationDbContext context) : ICommandHandler<UpdateTaskItemCommand, UpdateTaskItemResultDto>
{
    public async ValueTask<Result<UpdateTaskItemResultDto>> Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
    {
        var taskItem = await context.Tasks.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (taskItem is null)
            return Result.NotFound();

        if (request.Title is not null) taskItem.Title = request.Title;
        if (request.Notes is not null) taskItem.Notes = request.Notes;
        if (request.DueDate.HasValue) taskItem.DueDate = request.DueDate.Value;
        if (request.StartDate.HasValue) taskItem.StartDate = request.StartDate.Value;
        if (request.GoalId.HasValue) taskItem.GoalId = request.GoalId.Value;
        if (request.ProjectId.HasValue) taskItem.ProjectId = request.ProjectId.Value;
        if (request.ParentTaskId.HasValue) taskItem.ParentTaskId = request.ParentTaskId.Value;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateTaskItemResultDto(taskItem.Id, taskItem.Title));
    }
}