using Application.Common.Extensions;

namespace Application.Features.Tags;

public sealed record DeleteTagCommand(Guid Id) : ICommand<Unit>;

public class DeleteTagCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<DeleteTagCommand, Unit>
{
    public async ValueTask<Result<Unit>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var tag = await context.Tags
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tag is null)
            return Result.NotFound();

        context.Tags.Remove(tag);
        await context.SaveChangesAsync(cancellationToken);

        return Result.NoContent();
    }
}
