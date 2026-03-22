using FluentValidation;

namespace Application.Features.TaskItems;

public class UpdateTaskItemCommandValidator : AbstractValidator<UpdateTaskItemCommand>
{
    public UpdateTaskItemCommandValidator()
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

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.Now)
            .When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be in the future");

        RuleFor(x => x.StartDate)
            .LessThan(x => x.DueDate)
            .When(x => x.StartDate.HasValue && x.DueDate.HasValue)
            .WithMessage("Start date must be before due date");

        RuleFor(x => x.GoalId)
            .NotEmpty()
            .When(x => x.GoalId.HasValue)
            .WithMessage("GoalId must be a valid GUID");

        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .When(x => x.ProjectId.HasValue)
            .WithMessage("ProjectId must be a valid GUID");

        RuleFor(x => x.ParentTaskId)
            .NotEmpty()
            .When(x => x.ParentTaskId.HasValue)
            .WithMessage("ParentTaskId must be a valid GUID");
    }
}