using FluentValidation;

namespace Application.Features.GoalLogs;

public class CreateGoalLogCommandValidator : AbstractValidator<CreateGoalLogCommand>
{
    public CreateGoalLogCommandValidator()
    {
        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Date cannot be in the future");

        RuleFor(x => x.GoalId)
            .NotEmpty()
            .WithMessage("GoalId is required");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes must not exceed 500 characters");
    }
}