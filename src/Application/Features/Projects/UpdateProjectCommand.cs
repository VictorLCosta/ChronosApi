using Application.Common.Extensions;

namespace Application.Features.Projects;

public sealed record UpdateProjectResultDto(Guid Id, string Title);

public sealed record UpdateProjectCommand(Guid Id, string Title) : ICommand<UpdateProjectResultDto>;

public class UpdateProjectCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<UpdateProjectCommand, UpdateProjectResultDto>
{
    public async ValueTask<Result<UpdateProjectResultDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var project = await context.Projects
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project is null)
            return Result.NotFound();

        project.Title = request.Title;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateProjectResultDto(project.Id, project.Title));
    }
}
