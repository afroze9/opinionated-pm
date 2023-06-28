using FluentValidation;

namespace Nexus.CompanyAPI.Entities.Validations;

public class CompanyValidator : AbstractValidator<Company>
{
    public CompanyValidator()
    {
        RuleFor(c => c.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255);
    }
}