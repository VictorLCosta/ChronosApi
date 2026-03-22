using FluentValidation;

namespace Application.Features.Tags;

public class GetTagByIdQueryValidator : AbstractValidator<GetTagByIdQuery>
{
    public GetTagByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}