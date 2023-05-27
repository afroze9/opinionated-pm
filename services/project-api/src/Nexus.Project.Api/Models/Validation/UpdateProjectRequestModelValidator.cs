using FluentValidation;
using Nexus.ProjectAPI.Data;
using Nexus.ProjectAPI.Entities;

namespace Nexus.ProjectAPI.Models.Validation;

[ExcludeFromCodeCoverage]
public class UpdateProjectRequestModelValidator : AbstractValidator<UpdateProjectRequestModel>
{
    public UpdateProjectRequestModelValidator(UnitOfWork unitOfWork)
    {
        RuleFor(p => p.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255)
            .MustAsync(async (model, name, cancellationToken) =>
            {
                Project? existingProject = await unitOfWork.Projects.GetByNameAsync(name, cancellationToken);
                return existingProject == null || existingProject.Id == model.Id;
            })
            .WithMessage("Project with this name already exists");
    }
}