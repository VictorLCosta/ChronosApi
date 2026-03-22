namespace Application.Features.Reminders;

public sealed record DeleteReminderCommand(Guid Id) : ICommand<Unit>;

public class DeleteReminderCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteReminderCommand, Unit>
{
    public async ValueTask<Result<Unit>> Handle(DeleteReminderCommand request, CancellationToken cancellationToken)
    {
        var reminder = await context.Reminders.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (reminder is null)
            return Result.NotFound();

        context.Reminders.Remove(reminder);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}