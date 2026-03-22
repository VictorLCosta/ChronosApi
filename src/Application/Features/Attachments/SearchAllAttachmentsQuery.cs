using Application.Common.Extensions;

namespace Application.Features.Attachments;

public class SearchAllAttachmentsQuery : IQuery<PagedResponse<AttachmentDto>>, IPagedQuery
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Sort { get; set; }
};

public class SearchAllAttachmentsQueryHandler(IApplicationDbContext context) : IQueryHandler<SearchAllAttachmentsQuery, PagedResponse<AttachmentDto>>
{
    public async ValueTask<Result<PagedResponse<AttachmentDto>>> Handle(SearchAllAttachmentsQuery request, CancellationToken cancellationToken)
    {
        var attachments = await context.Attachments
            .Select(a => new AttachmentDto(a.Id, a.FileName, a.ContentType, a.SizeBytes, a.StorageUrl, a.TaskItemId))
            .ToPagedResponseAsync(request, cancellationToken);

        return Result.Success(attachments);
    }
}