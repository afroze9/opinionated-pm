using FluentValidation;
using Nexus.CompanyAPI.Data.Repositories;

namespace Nexus.CompanyAPI.Model.Validation;

[ExcludeFromCodeCoverage]
public class CompanyRequestModelValidator : AbstractValidator<CompanyRequestModel>
{
    public CompanyRequestModelValidator()
    {
        RuleFor(c => c.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255);
    }
}