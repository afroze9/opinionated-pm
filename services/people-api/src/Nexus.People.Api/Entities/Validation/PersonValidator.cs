using FluentValidation;

namespace Nexus.PeopleAPI.Entities.Validation;

[ExcludeFromCodeCoverage]
public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(p => p.Email)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255)
            .EmailAddress();

        RuleFor(p => p.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255);

        RuleFor(p => p.Password)
            .NotNull()
            .MinimumLength(8);
    }
}