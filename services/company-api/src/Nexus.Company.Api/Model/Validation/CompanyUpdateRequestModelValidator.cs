using FluentValidation;
using Nexus.CompanyAPI.Data.Repositories;
using Nexus.CompanyAPI.Entities;

namespace Nexus.CompanyAPI.Model.Validation;

[ExcludeFromCodeCoverage]
public class CompanyUpdateRequestModelValidator : AbstractValidator<CompanyUpdateRequestModel>
{
    public CompanyUpdateRequestModelValidator()
    {
        RuleFor(c => c.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255);
    }
}