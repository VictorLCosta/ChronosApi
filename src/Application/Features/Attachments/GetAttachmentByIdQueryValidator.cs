using FluentValidation;

namespace Application.Features.Attachments;

public class GetAttachmentByIdQueryValidator : AbstractValidator<GetAttachmentByIdQuery>
{
    public GetAttachmentByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}