using Application.Common.Extensions;

namespace Application.Features.TaskItems;

public sealed record TaskItemDto(Guid Id, string Title, string? Notes, DateTime? DueDate, DateTime? StartDate, Guid? GoalId, Guid? ProjectId, Guid? ParentTaskId);

public class SearchAllTaskItemsQuery : IQuery<PagedResponse<TaskItemDto>>, IPagedQuery
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }
};

public class SearchAllTaskItemsQueryHandler(IApplicationDbContext context) : IQueryHandler<SearchAllTaskItemsQuery, PagedResponse<TaskItemDto>>
{
    public async ValueTask<Result<PagedResponse<TaskItemDto>>> Handle(SearchAllTaskItemsQuery request, CancellationToken cancellationToken)
    {
        var taskItems = await context.Tasks
            .Select(t => new TaskItemDto(t.Id, t.Title, t.Notes, t.DueDate, t.StartDate, t.GoalId, t.ProjectId, t.ParentTaskId))
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(taskItems);
    }
}