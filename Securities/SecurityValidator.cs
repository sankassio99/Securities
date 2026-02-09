using FluentValidation;

namespace Securities;

public class SecurityValidator : AbstractValidator<ExecuteSecurityRequest>
{
    public SecurityValidator()
    {
        RuleFor(security => security.Isins)
            .NotEmpty()
            .WithMessage("ISIN must not be empty!");
    }
}
