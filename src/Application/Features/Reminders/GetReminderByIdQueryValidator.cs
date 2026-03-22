using FluentValidation;

namespace Application.Features.Reminders;

public class GetReminderByIdQueryValidator : AbstractValidator<GetReminderByIdQuery>
{
    public GetReminderByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}