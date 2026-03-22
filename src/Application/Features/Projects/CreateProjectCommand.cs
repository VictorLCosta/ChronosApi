namespace Application.Features.Projects;

public sealed record CreateProjectResultDto(Guid Id, string Title);

public sealed record CreateProjectCommand(string Title) : ICommand<CreateProjectResultDto>;

public class CreateProjectCommandHandler(IApplicationDbContext context) : ICommandHandler<CreateProjectCommand, CreateProjectResultDto>
{
    public async ValueTask<Result<CreateProjectResultDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await context.Projects.AddAsync(new Domain.Entities.Project
        {
            Title = request.Title
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result<CreateProjectResultDto>.Created(
            new CreateProjectResultDto(project.Entity.Id, project.Entity.Title)
        );
    }
}