namespace Application.Features.Attachments;

public sealed record UpdateAttachmentResultDto(Guid Id);

public sealed record UpdateAttachmentCommand(
    Guid Id,
    string? FileName = null,
    string? ContentType = null,
    long? SizeBytes = null,
    string? StorageUrl = null,
    Guid? TaskItemId = null
) : ICommand<UpdateAttachmentResultDto>;

public class UpdateAttachmentCommandHandler(IApplicationDbContext context) : ICommandHandler<UpdateAttachmentCommand, UpdateAttachmentResultDto>
{
    public async ValueTask<Result<UpdateAttachmentResultDto>> Handle(UpdateAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await context.Attachments.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (attachment is null)
            return Result.NotFound();

        if (request.FileName is not null) attachment.FileName = request.FileName;
        if (request.ContentType is not null) attachment.ContentType = request.ContentType;
        if (request.SizeBytes.HasValue) attachment.SizeBytes = request.SizeBytes.Value;
        if (request.StorageUrl is not null) attachment.StorageUrl = request.StorageUrl;
        if (request.TaskItemId.HasValue) attachment.TaskItemId = request.TaskItemId.Value;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateAttachmentResultDto(attachment.Id));
    }
}