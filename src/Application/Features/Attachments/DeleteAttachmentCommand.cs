using Application.Common.Extensions;

namespace Application.Features.Attachments;

public sealed record DeleteAttachmentCommand(Guid Id) : ICommand<Unit>;

public class DeleteAttachmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<DeleteAttachmentCommand, Unit>
{
    public async ValueTask<Result<Unit>> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var attachment = await context.Attachments
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (attachment is null)
            return Result.NotFound();

        context.Attachments.Remove(attachment);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
