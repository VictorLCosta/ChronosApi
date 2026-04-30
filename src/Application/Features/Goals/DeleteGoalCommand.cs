using Application.Common.Extensions;

namespace Application.Features.Goals;

public sealed record DeleteGoalCommand(Guid Id) : ICommand<Unit>;

public class DeleteGoalCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) : ICommandHandler<DeleteGoalCommand, Unit>
{
    public async ValueTask<Result<Unit>> Handle(DeleteGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var goal = await context.Goals
            .WhereCreatedBy(userId)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (goal is null)
            return Result.NotFound();

        context.Goals.Remove(goal);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
