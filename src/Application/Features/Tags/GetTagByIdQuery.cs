using Application.Common.Extensions;

namespace Application.Features.Tags;

public sealed record GetTagByIdQuery(Guid Id) : IQuery<TagDto?>, ICacheable
{
    public bool BypassCache => true;
    public string CacheKey => $"GetTagByIdQuery:{Id}";
    public int SlidingExpirationInMinutes => 5;
    public int AbsoluteExpirationInMinutes => 5;
};

public class GetTagByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : IQueryHandler<GetTagByIdQuery, TagDto?>
{
    public async ValueTask<Result<TagDto?>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var tag = await context.Tags
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tag is null)
            return Result.NotFound();

        var tagDto = new TagDto(tag.Id, tag.Name);

        return Result.Success<TagDto?>(tagDto);
    }
}
