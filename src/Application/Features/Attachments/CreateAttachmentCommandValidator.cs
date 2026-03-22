using FluentValidation;

namespace Application.Features.Attachments;

public class CreateAttachmentCommandValidator : AbstractValidator<CreateAttachmentCommand>
{
    public CreateAttachmentCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("FileName is required")
            .MaximumLength(255)
            .WithMessage("FileName must not exceed 255 characters");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("ContentType is required")
            .MaximumLength(100)
            .WithMessage("ContentType must not exceed 100 characters");

        RuleFor(x => x.SizeBytes)
            .GreaterThan(0)
            .WithMessage("SizeBytes must be greater than 0")
            .LessThanOrEqualTo(50 * 1024 * 1024) // 50MB
            .WithMessage("File size must not exceed 50MB");

        RuleFor(x => x.StorageUrl)
            .NotEmpty()
            .WithMessage("StorageUrl is required")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("StorageUrl must be a valid URL");

        RuleFor(x => x.TaskItemId)
            .NotEmpty()
            .WithMessage("TaskItemId is required");
    }
}