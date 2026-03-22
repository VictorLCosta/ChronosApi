namespace Application.Features.TaskItems;

public sealed record CreateTaskItemResultDto(Guid Id, string Title);

public sealed record CreateTaskItemCommand(
    string Title,
    string? Notes = null,
    DateTime? DueDate = null,
    DateTime? StartDate = null,
    Guid? GoalId = null,
    Guid? ProjectId = null,
    Guid? ParentTaskId = null
) : ICommand<CreateTaskItemResultDto>;

public class CreateTaskItemCommandHandler(IApplicationDbContext context) : ICommandHandler<CreateTaskItemCommand, CreateTaskItemResultDto>
{
    public async ValueTask<Result<CreateTaskItemResultDto>> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
    {
        var taskItem = await context.Tasks.AddAsync(new Domain.Entities.TaskItem
        {
            Title = request.Title,
            Notes = request.Notes,
            DueDate = request.DueDate,
            StartDate = request.StartDate,
            GoalId = request.GoalId,
            ProjectId = request.ProjectId ?? Guid.Empty,
            ParentTaskId = request.ParentTaskId
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result<CreateTaskItemResultDto>.Created(
            new CreateTaskItemResultDto(taskItem.Entity.Id, taskItem.Entity.Title)
        );
    }
}