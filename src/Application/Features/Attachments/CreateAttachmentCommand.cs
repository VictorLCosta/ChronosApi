using Application.Common.Extensions;

namespace Application.Features.Attachments;

public sealed record AttachmentDto(Guid Id, string FileName, string ContentType, long SizeBytes, string StorageUrl, Guid TaskItemId);

public sealed record CreateAttachmentResultDto(Guid Id);

public sealed record CreateAttachmentCommand(
    string FileName,
    string ContentType,
    long SizeBytes,
    string StorageUrl,
    Guid TaskItemId
) : ICommand<CreateAttachmentResultDto>;

public class CreateAttachmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<CreateAttachmentCommand, CreateAttachmentResultDto>
{
    public async ValueTask<Result<CreateAttachmentResultDto>> Handle(CreateAttachmentCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var attachment = await context.Attachments.AddAsync(new Domain.Entities.Attachment
        {
            FileName = request.FileName,
            ContentType = request.ContentType,
            SizeBytes = request.SizeBytes,
            StorageUrl = request.StorageUrl,
            TaskItemId = request.TaskItemId,
            CreatedBy = userId
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result<CreateAttachmentResultDto>.Created(new CreateAttachmentResultDto(attachment.Entity.Id));
    }
}
