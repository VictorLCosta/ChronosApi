using Application.Common.Extensions;

namespace Application.Features.TaskItems;

public sealed record GetTaskItemByIdQuery(Guid Id) : IQuery<TaskItemDto?>, ICacheable
{
    public bool BypassCache => true;
    public string CacheKey => $"GetTaskItemByIdQuery:{Id}";
    public int SlidingExpirationInMinutes => 5;
    public int AbsoluteExpirationInMinutes => 5;
};

public class GetTaskItemByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : IQueryHandler<GetTaskItemByIdQuery, TaskItemDto?>
{
    public async ValueTask<Result<TaskItemDto?>> Handle(GetTaskItemByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var taskItem = await context.Tasks
            .AsNoTracking()
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (taskItem is null)
            return Result.NotFound();

        var taskItemDto = new TaskItemDto(taskItem.Id, taskItem.Title, taskItem.Notes, taskItem.DueDate, taskItem.StartDate, taskItem.GoalId, taskItem.ProjectId, taskItem.ParentTaskId);

        return Result.Success<TaskItemDto?>(taskItemDto);
    }
}
