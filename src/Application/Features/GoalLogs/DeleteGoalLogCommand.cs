using Application.Common.Extensions;

namespace Application.Features.GoalLogs;

public sealed record DeleteGoalLogCommand(Guid Id) : ICommand<Unit>;

public class DeleteGoalLogCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<DeleteGoalLogCommand, Unit>
{
    public async ValueTask<Result<Unit>> Handle(DeleteGoalLogCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var goalLog = await context.GoalLogs
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(gl => gl.Id == request.Id, cancellationToken);

        if (goalLog is null)
            return Result.NotFound();

        context.GoalLogs.Remove(goalLog);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
