using Domain.Enums;

using FluentValidation;

namespace Application.Features.Goals;

public class CreateGoalCommandValidator : AbstractValidator<CreateGoalCommand>
{
    public CreateGoalCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes must not exceed 1000 characters");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid status value");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Invalid priority value");

        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .When(x => x.ProjectId.HasValue)
            .WithMessage("ProjectId must be a valid GUID");
    }
}