using FluentValidation;
using ProjectManagement.CompanyAPI.Data.Repositories;
using ProjectManagement.CompanyAPI.Domain.Entities;

namespace ProjectManagement.CompanyAPI.Model.Validation;

[ExcludeFromCodeCoverage]
public class CompanyUpdateRequestModelValidator : AbstractValidator<CompanyUpdateRequestModel>
{
    public CompanyUpdateRequestModelValidator(CompanyRepository repository)
    {
        RuleFor(c => c.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255)
            .MustAsync(async (model, name, cancellationToken) =>
            {
                Company? existingCompany =
                    await repository.GetByNameAsync(name, cancellationToken);

                return existingCompany == null || existingCompany.Id == model.Id;
            })
            .WithMessage("Company with this name already exists");
    }
}