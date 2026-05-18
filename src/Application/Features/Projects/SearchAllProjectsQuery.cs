using CrossCutting.ExtensionMethods;

namespace Application.Features.Projects;

public sealed record ProjectDto(Guid Id, string Title);

public class SearchAllProjectsQuery : IQuery<PagedResponse<ProjectDto>>, IPagedQuery, ICacheable
{
    public int? PageNumber { get; set; } = 10;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }
    public string? Q { get; set; }

    public bool BypassCache => false;

    public string CacheKey => $"SearchAllProjectsQuery:{PageNumber}:{PageSize}:{Sort}:{Q}";

    public int SlidingExpirationInMinutes => 5;

    public int AbsoluteExpirationInMinutes => 5;

    public IReadOnlyCollection<string> CacheTags => ["Projects"];
};

public class SearchAllProjectsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : IQueryHandler<SearchAllProjectsQuery, PagedResponse<ProjectDto>>
{
    public async ValueTask<Result<PagedResponse<ProjectDto>>> Handle(SearchAllProjectsQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var searchQuery = request.Q.NormalizeSearchQuery();

        var userId = currentUserService.GetRequiredUserId();

        var projects = await context.Projects
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(request.Q), p => p.SearchVector.Matches(EF.Functions.PlainToTsQuery(searchQuery)))
            .Where(p => p.CreatedBy == userId)
            .OrderByDescending(p => p.Created)
            .Select(p => new ProjectDto(p.Id, p.Title))
            .ApplySort(request.Sort)
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(projects);
    }
}