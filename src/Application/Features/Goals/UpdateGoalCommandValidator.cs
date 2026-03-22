using Domain.Enums;

using FluentValidation;

namespace Application.Features.Goals;

public class UpdateGoalCommandValidator : AbstractValidator<UpdateGoalCommand>
{
    public UpdateGoalCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .When(x => x.Title is not null)
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => x.Notes is not null)
            .WithMessage("Notes must not exceed 1000 characters");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Invalid status value");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .When(x => x.Priority.HasValue)
            .WithMessage("Invalid priority value");

        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .When(x => x.ProjectId.HasValue)
            .WithMessage("ProjectId must be a valid GUID");
    }
}