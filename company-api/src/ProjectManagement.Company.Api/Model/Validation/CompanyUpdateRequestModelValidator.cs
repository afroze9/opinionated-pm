using FluentValidation;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Domain.Entities;
using ProjectManagement.CompanyAPI.Domain.Specifications;

namespace ProjectManagement.CompanyAPI.Model.Validation;

[ExcludeFromCodeCoverage]
public class CompanyUpdateRequestModelValidator : AbstractValidator<CompanyUpdateRequestModel>
{
    public CompanyUpdateRequestModelValidator(IRepository<Company> repository)
    {
        RuleFor(c => c.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255)
            .MustAsync(async (model, name, cancellationToken) =>
            {
                Company? existingCompany =
                    await repository.FirstOrDefaultAsync(new CompanyByNameSpec(name), cancellationToken);

                return existingCompany == null || existingCompany.Id == model.Id;
            })
            .WithMessage("Company with this name already exists");
    }
}