using Application.Common.Extensions;

namespace Application.Features.Projects;

public sealed record CreateProjectResultDto(Guid Id, string Title);

public sealed record CreateProjectCommand(string Title) : ICommand<CreateProjectResultDto>;

public class CreateProjectCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<CreateProjectCommand, CreateProjectResultDto>
{
    public async ValueTask<Result<CreateProjectResultDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var project = await context.Projects.AddAsync(new Domain.Entities.Project
        {
            Title = request.Title,
            CreatedBy = userId
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result<CreateProjectResultDto>.Created(
            new CreateProjectResultDto(project.Entity.Id, project.Entity.Title)
        );
    }
}
