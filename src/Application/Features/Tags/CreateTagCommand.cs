namespace Application.Features.Tags;

public sealed record TagDto(Guid Id, string Name);

public sealed record CreateTagResultDto(Guid Id, string Name);

public sealed record CreateTagCommand(string Name) : ICommand<CreateTagResultDto>;

public class CreateTagCommandHandler(IApplicationDbContext context) : ICommandHandler<CreateTagCommand, CreateTagResultDto>
{
    public async ValueTask<Result<CreateTagResultDto>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await context.Tags.AddAsync(new Domain.Entities.Tag { Name = request.Name }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result<CreateTagResultDto>.Created(new CreateTagResultDto(tag.Entity.Id, tag.Entity.Name));
    }
}