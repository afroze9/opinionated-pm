using FluentValidation;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Domain.Entities;
using ProjectManagement.CompanyAPI.Domain.Specifications;

namespace ProjectManagement.CompanyAPI.Model.Validation;

[ExcludeFromCodeCoverage]
public class CompanyRequestModelValidator : AbstractValidator<CompanyRequestModel>
{
    public CompanyRequestModelValidator(IRepository<Company> repository)
    {
        RuleFor(c => c.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255)
            .MustAsync(async (name, cancellationToken) =>
            {
                bool exists = await repository.AnyAsync(new CompanyByNameSpec(name), cancellationToken);
                return !exists;
            })
            .WithMessage("Company with this name already exists");
    }
}