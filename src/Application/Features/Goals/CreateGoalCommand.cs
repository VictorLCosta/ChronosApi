using Application.Common.Extensions;

using Domain.Enums;

namespace Application.Features.Goals;

public sealed record CreateGoalResultDto(Guid Id, string Title);

public sealed record CreateGoalCommand(
    string Title,
    string? Notes = null,
    GoalStatus Status = GoalStatus.NotStarted,
    PriorityLevel Priority = PriorityLevel.Medium,
    Guid? ProjectId = null
) : ICommand<CreateGoalResultDto>;

public class CreateGoalCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<CreateGoalCommand, CreateGoalResultDto>
{
    public async ValueTask<Result<CreateGoalResultDto>> Handle(CreateGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var goal = await context.Goals.AddAsync(new Domain.Entities.Goal
        {
            Title = request.Title,
            Notes = request.Notes,
            Status = request.Status,
            Priority = request.Priority,
            ProjectId = request.ProjectId,
            CreatedBy = userId
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result<CreateGoalResultDto>.Created(
            new CreateGoalResultDto(goal.Entity.Id, goal.Entity.Title)
        );
    }
}
