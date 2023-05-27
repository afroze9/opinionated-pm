using FluentValidation;
using ProjectManagement.ProjectAPI.Data;
using ProjectManagement.ProjectAPI.Entities;

namespace ProjectManagement.ProjectAPI.Models.Validation;

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