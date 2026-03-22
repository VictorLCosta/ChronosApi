using Application.Common.Extensions;

namespace Application.Features.Tags;

public class SearchAllTagsQuery : IQuery<PagedResponse<TagDto>>, IPagedQuery
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }
};

public class SearchAllTagsQueryHandler(IApplicationDbContext context) : IQueryHandler<SearchAllTagsQuery, PagedResponse<TagDto>>
{
    public async ValueTask<Result<PagedResponse<TagDto>>> Handle(SearchAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await context.Tags
            .Select(t => new TagDto(t.Id, t.Name))
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(tags);
    }
}