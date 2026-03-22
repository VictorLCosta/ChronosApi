using FluentValidation;

namespace Application.Features.Tags;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$")
            .WithMessage("Name can only contain letters, numbers, spaces, hyphens, and underscores");
    }
}