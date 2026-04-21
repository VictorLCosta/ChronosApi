using FluentValidation;

namespace Application.Features.Identity.TwoFactor;

public sealed class VerifyTwoFactorLoginCommandValidator : AbstractValidator<VerifyTwoFactorLoginCommand>
{
    public VerifyTwoFactorLoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Authenticator code is required.")
            .Length(6).WithMessage("Authenticator code must contain 6 digits.")
            .Matches("^[0-9]{6}$").WithMessage("Authenticator code must contain only digits.");
    }
}