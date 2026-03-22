namespace Application.Features.Projects;

public sealed record DeleteProjectCommand(Guid Id) : ICommand<Unit>;

public class DeleteProjectCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteProjectCommand, Unit>
{
    public async ValueTask<Result<Unit>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await context.Projects.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project is null)
            return Result.NotFound();

        context.Projects.Remove(project);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}