namespace Application.Features.Projects;

public sealed record GetProjectByIdQuery(Guid Id) : IQuery<ProjectDto?>, ICacheable
{
    public bool BypassCache => false;

    public string CacheKey => $"GetProjectByIdQuery:{Id}";

    public int SlidingExpirationInMinutes => 10;

    public int AbsoluteExpirationInMinutes => 60;
}

public class GetProjectByIdQueryHandler(IApplicationDbContext context) : IQueryHandler<GetProjectByIdQuery, ProjectDto?>
{
    public async ValueTask<Result<ProjectDto?>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project is null)
            return Result.NotFound();

        var projectDto = new ProjectDto(project.Id, project.Title);

        return Result.Success<ProjectDto?>(projectDto);
    }
}