using FluentValidation;

namespace Application.Features.Identity.Tokens.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(p => p.Token)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(p => p.RefreshToken)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}