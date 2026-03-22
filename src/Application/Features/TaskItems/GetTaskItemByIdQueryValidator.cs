using FluentValidation;

namespace Application.Features.TaskItems;

public class GetTaskItemByIdQueryValidator : AbstractValidator<GetTaskItemByIdQuery>
{
    public GetTaskItemByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}