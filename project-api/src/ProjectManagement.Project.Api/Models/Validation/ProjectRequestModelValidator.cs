using FluentValidation;
using ProjectManagement.ProjectAPI.Data;

namespace ProjectManagement.ProjectAPI.Models.Validation;

[ExcludeFromCodeCoverage]
public class ProjectRequestModelValidator : AbstractValidator<ProjectRequestModel>
{
    public ProjectRequestModelValidator(UnitOfWork unitOfWork)
    {
        RuleFor(p => p.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255)
            .MustAsync(async (name, cancellationToken) =>
            {
                bool exists = await unitOfWork.Projects.AnyByNameAsync(name, cancellationToken);
                return !exists;
            })
            .WithMessage("Project with this name already exists");
    }
}