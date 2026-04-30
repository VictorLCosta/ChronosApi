using Application.Common.Extensions;

namespace Application.Features.GoalLogs;

public sealed record GoalLogDto(Guid Id, DateOnly Date, string? Notes, bool? Completed, Guid GoalId);

public sealed record CreateGoalLogResultDto(Guid Id);

public sealed record CreateGoalLogCommand(
    DateOnly Date,
    Guid GoalId,
    string? Notes = null,
    bool? Completed = null
) : ICommand<CreateGoalLogResultDto>;

public class CreateGoalLogCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<CreateGoalLogCommand, CreateGoalLogResultDto>
{
    public async ValueTask<Result<CreateGoalLogResultDto>> Handle(CreateGoalLogCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var goalLog = await context.GoalLogs.AddAsync(new Domain.Entities.GoalLog
        {
            Date = request.Date,
            Notes = request.Notes,
            Completed = request.Completed,
            GoalId = request.GoalId,
            CreatedBy = userId
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result<CreateGoalLogResultDto>.Created(new CreateGoalLogResultDto(goalLog.Entity.Id));
    }
}
