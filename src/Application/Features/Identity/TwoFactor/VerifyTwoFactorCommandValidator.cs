using FluentValidation;

namespace Application.Features.Identity.TwoFactor;

public sealed class VerifyTwoFactorCommandValidator : AbstractValidator<VerifyTwoFactorCommand>
{
    public VerifyTwoFactorCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Authenticator code is required.")
            .Length(6).WithMessage("Authenticator code must contain 6 digits.")
            .Matches("^[0-9]{6}$").WithMessage("Authenticator code must contain only digits.");
    }
}