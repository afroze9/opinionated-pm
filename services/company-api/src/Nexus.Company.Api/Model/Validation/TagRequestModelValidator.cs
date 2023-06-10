using FluentValidation;
using Nexus.CompanyAPI.Data.Repositories;

namespace Nexus.CompanyAPI.Model.Validation;

[ExcludeFromCodeCoverage]
public class TagRequestModelValidator : AbstractValidator<TagRequestModel>
{
    public TagRequestModelValidator()
    {
        RuleFor(c => c.Name)
            .NotNull()
            .MaximumLength(20);
    }
}