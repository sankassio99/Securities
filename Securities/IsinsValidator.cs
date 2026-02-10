using FluentValidation;

namespace Securities;

public class IsinsValidator : AbstractValidator<ExecuteSecurityRequest>
{
    // TODO: Write a new setup file
    public IsinsValidator()
    {
        RuleFor(security => security.Isins)
            .NotEmpty()
            .WithMessage("ISIN must not be empty!")
            .Must(isin => isin.All(i => i.Length == 12))
            .WithMessage("All ISINs must be exactly 12 characters long!");
    }
}
