using Application.Common.Extensions;

using Domain.Enums;

namespace Application.Features.Goals;

public sealed record UpdateGoalResultDto(Guid Id, string Title);

public sealed record UpdateGoalCommand(
    Guid Id,
    string Title,
    string? Notes = null,
    GoalStatus? Status = null,
    PriorityLevel? Priority = null,
    Guid? ProjectId = null
) : ICommand<UpdateGoalResultDto>;

public class UpdateGoalCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<UpdateGoalCommand, UpdateGoalResultDto>
{
    public async ValueTask<Result<UpdateGoalResultDto>> Handle(UpdateGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var goal = await context.Goals
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (goal is null)
            return Result.NotFound();

        if (request.Title is not null) goal.Title = request.Title;
        if (request.Notes is not null) goal.Notes = request.Notes;
        if (request.Status.HasValue) goal.Status = request.Status.Value;
        if (request.Priority.HasValue) goal.Priority = request.Priority.Value;
        if (request.ProjectId.HasValue) goal.ProjectId = request.ProjectId.Value;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateGoalResultDto(goal.Id, goal.Title));
    }
}
