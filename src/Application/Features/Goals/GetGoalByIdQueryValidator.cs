using FluentValidation;

namespace Application.Features.Goals;

public class GetGoalByIdQueryValidator : AbstractValidator<GetGoalByIdQuery>
{
    public GetGoalByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}