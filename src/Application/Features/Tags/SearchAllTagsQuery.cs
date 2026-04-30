using Application.Common.Extensions;

namespace Application.Features.Tags;

public class SearchAllTagsQuery : IQuery<PagedResponse<TagDto>>, IPagedQuery, ICacheable
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }

    public bool BypassCache => false;
    public string CacheKey => $"SearchAllTagsQuery:{PageNumber}:{PageSize}:{Sort}";
    public int SlidingExpirationInMinutes => 5;
    public int AbsoluteExpirationInMinutes => 5;
};

public class SearchAllTagsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : IQueryHandler<SearchAllTagsQuery, PagedResponse<TagDto>>
{
    public async ValueTask<Result<PagedResponse<TagDto>>> Handle(SearchAllTagsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var tags = await context.Tags
            .AsNoTracking()
            .WhereCreatedBy(userId)
            .Select(t => new TagDto(t.Id, t.Name))
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(tags);
    }
}
