using FluentValidation;

namespace Application.Features.Reminders;

public class CreateReminderCommandValidator : AbstractValidator<CreateReminderCommand>
{
    public CreateReminderCommandValidator()
    {
        RuleFor(x => x.RemindAt)
            .GreaterThan(DateTime.Now)
            .WithMessage("Reminder date must be in the future");

        RuleFor(x => x.OffsetMinutes)
            .GreaterThan(0)
            .When(x => x.OffsetMinutes.HasValue)
            .WithMessage("Offset minutes must be positive");

        RuleFor(x => x.TaskItemId)
            .NotEmpty()
            .When(x => x.TaskItemId.HasValue)
            .WithMessage("TaskItemId must be a valid GUID");

        RuleFor(x => x.GoalId)
            .NotEmpty()
            .When(x => x.GoalId.HasValue)
            .WithMessage("GoalId must be a valid GUID");

        RuleFor(x => x)
            .Must(x => x.TaskItemId.HasValue || x.GoalId.HasValue)
            .WithMessage("Either TaskItemId or GoalId must be provided");
    }
}