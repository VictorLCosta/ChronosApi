using Application.Common.Extensions;

namespace Application.Features.Projects;

public sealed record ProjectDto(Guid Id, string Title);

public class SearchAllProjectsQuery : IQuery<PagedResponse<ProjectDto>>, IPagedQuery
{
    public int? PageNumber { get; set; } = 10;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }
};

public class SearchAllProjectsQueryHandler(IApplicationDbContext context) : IQueryHandler<SearchAllProjectsQuery, PagedResponse<ProjectDto>>
{
    public async ValueTask<Result<PagedResponse<ProjectDto>>> Handle(SearchAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await context.Projects
            .Select(p => new ProjectDto(p.Id, p.Title))
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(projects);
    }
}