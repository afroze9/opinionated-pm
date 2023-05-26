using FluentValidation;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Domain.Entities;
using ProjectManagement.CompanyAPI.Domain.Specifications;

namespace ProjectManagement.CompanyAPI.Model.Validation;

[ExcludeFromCodeCoverage]
public class TagRequestModelValidator : AbstractValidator<TagRequestModel>
{
    public TagRequestModelValidator(IRepository<Tag> repository)
    {
        RuleFor(c => c.Name)
            .NotNull()
            .MaximumLength(20)
            .MustAsync(async (name, cancellationToken) =>
            {
                bool exists = await repository.AnyAsync(new TagByNameSpec(name), cancellationToken);
                return !exists;
            })
            .WithMessage("Tag with this name already exists");
    }
}