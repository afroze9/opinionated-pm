using FluentValidation;
using PeopleAPI.Data.Repositories;

namespace PeopleAPI.Model.Validation;

[ExcludeFromCodeCoverage]
public class PersonCreateRequestModelValidator : AbstractValidator<PersonCreateRequestModel>
{
    public PersonCreateRequestModelValidator(PeopleRepository repository)
    {
        RuleFor(c => c.Email)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255)
            .EmailAddress()
            .MustAsync(async (email, cancellationToken) =>
            {
                bool exists = await repository.ExistsWithEmailAsync(email, cancellationToken);
                return !exists;
            })
            .WithMessage("Person with this email already exists");
    }
}