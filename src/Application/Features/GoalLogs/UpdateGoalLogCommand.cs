namespace Application.Features.GoalLogs;

public sealed record UpdateGoalLogResultDto(Guid Id);

public sealed record UpdateGoalLogCommand(
    Guid Id,
    DateOnly? Date = null,
    string? Notes = null,
    bool? Completed = null,
    Guid? GoalId = null
) : ICommand<UpdateGoalLogResultDto>;

public class UpdateGoalLogCommandHandler(IApplicationDbContext context) : ICommandHandler<UpdateGoalLogCommand, UpdateGoalLogResultDto>
{
    public async ValueTask<Result<UpdateGoalLogResultDto>> Handle(UpdateGoalLogCommand request, CancellationToken cancellationToken)
    {
        var goalLog = await context.GoalLogs.FirstOrDefaultAsync(gl => gl.Id == request.Id, cancellationToken);

        if (goalLog is null)
            return Result.NotFound();

        if (request.Date.HasValue) goalLog.Date = request.Date.Value;
        if (request.Notes is not null) goalLog.Notes = request.Notes;
        if (request.Completed.HasValue) goalLog.Completed = request.Completed.Value;
        if (request.GoalId.HasValue) goalLog.GoalId = request.GoalId.Value;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateGoalLogResultDto(goalLog.Id));
    }
}