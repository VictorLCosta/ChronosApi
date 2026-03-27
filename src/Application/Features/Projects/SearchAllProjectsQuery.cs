using Application.Common.Extensions;

namespace Application.Features.Projects;

public sealed record ProjectDto(Guid Id, string Title);

public class SearchAllProjectsQuery : IQuery<PagedResponse<ProjectDto>>, IPagedQuery, ICacheable
{
    public int? PageNumber { get; set; } = 10;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }

    public bool BypassCache => false;

    public string CacheKey => "SearchAllProjectsQuery";

    public int SlidingExpirationInMinutes => 5;

    public int AbsoluteExpirationInMinutes => 5;
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