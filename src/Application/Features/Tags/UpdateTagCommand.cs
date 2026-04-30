using Application.Common.Extensions;

namespace Application.Features.Tags;

public sealed record UpdateTagResultDto(Guid Id, string Name);

public sealed record UpdateTagCommand(Guid Id, string Name) : ICommand<UpdateTagResultDto>;

public class UpdateTagCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<UpdateTagCommand, UpdateTagResultDto>
{
    public async ValueTask<Result<UpdateTagResultDto>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var tag = await context.Tags
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tag is null)
            return Result.NotFound();

        tag.Name = request.Name;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateTagResultDto(tag.Id, tag.Name));
    }
}
