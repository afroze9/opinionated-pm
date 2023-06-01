using FluentValidation;
using PeopleAPI.Data.Repositories;
using PeopleAPI.Entities;

namespace PeopleAPI.Model.Validation;

[ExcludeFromCodeCoverage]
public class PersonUpdateRequestModelValidator : AbstractValidator<PersonUpdateRequestModel>
{
    public PersonUpdateRequestModelValidator()
    {
        RuleFor(c => c.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255);
    }
}
