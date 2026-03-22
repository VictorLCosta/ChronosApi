using FluentValidation;

namespace Application.Features.Reminders;

public class UpdateReminderCommandValidator : AbstractValidator<UpdateReminderCommand>
{
    public UpdateReminderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(x => x.RemindAt)
            .GreaterThan(DateTime.Now)
            .When(x => x.RemindAt.HasValue)
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
    }
}