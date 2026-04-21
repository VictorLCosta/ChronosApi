using FluentValidation;

namespace Application.Features.Identity.Tokens.AccessToken;

public class GenerateAccessTokenCommandValidator : AbstractValidator<GenerateAccessTokenCommand>
{
    public GenerateAccessTokenCommandValidator()
    {
        RuleFor(p => p.Email)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .EmailAddress();

        RuleFor(p => p.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}