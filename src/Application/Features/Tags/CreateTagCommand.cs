using Application.Common.Extensions;

namespace Application.Features.Tags;

public sealed record TagDto(Guid Id, string Name);

public sealed record CreateTagResultDto(Guid Id, string Name);

public sealed record CreateTagCommand(string Name) : ICommand<CreateTagResultDto>;

public class CreateTagCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<CreateTagCommand, CreateTagResultDto>
{
    public async ValueTask<Result<CreateTagResultDto>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var tag = await context.Tags.AddAsync(new Domain.Entities.Tag
        {
            Name = request.Name,
            CreatedBy = userId
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result<CreateTagResultDto>.Created(new CreateTagResultDto(tag.Entity.Id, tag.Entity.Name));
    }
}
