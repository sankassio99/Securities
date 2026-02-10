using FluentValidation;

namespace Securities;

public class IsinsValidator : AbstractValidator<ExecuteSecurityRequest>
{
    // TODO: Write a new setup file
    public IsinsValidator()
    {
        RuleFor(security => security.Isins)
            .NotEmpty()
            .WithMessage("ISIN must not be empty!");
    }
}
