namespace Application.Features.Attachments;

public sealed record GetAttachmentByIdQuery(Guid Id) : IQuery<AttachmentDto?>;

public class GetAttachmentByIdQueryHandler(IApplicationDbContext context) : IQueryHandler<GetAttachmentByIdQuery, AttachmentDto?>
{
    public async ValueTask<Result<AttachmentDto?>> Handle(GetAttachmentByIdQuery request, CancellationToken cancellationToken)
    {
        var attachment = await context.Attachments
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (attachment is null)
            return Result.NotFound();

        var attachmentDto = new AttachmentDto(attachment.Id, attachment.FileName, attachment.ContentType, attachment.SizeBytes, attachment.StorageUrl, attachment.TaskItemId);

        return Result.Success<AttachmentDto?>(attachmentDto);
    }
}