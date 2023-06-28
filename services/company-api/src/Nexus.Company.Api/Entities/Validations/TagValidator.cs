using FluentValidation;

namespace Nexus.CompanyAPI.Entities.Validations;

public class TagValidator : AbstractValidator<Tag>
{
    public TagValidator()
    {
        RuleFor(c => c.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255);
    }
}