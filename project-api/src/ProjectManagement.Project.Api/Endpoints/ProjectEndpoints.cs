using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.ProjectAPI.Data;
using ProjectManagement.ProjectAPI.Entities;
using ProjectManagement.ProjectAPI.Models;

namespace ProjectManagement.ProjectAPI.Endpoints;

public static class ProjectEndpoints
{
    public static void AddProjectEndpoints(this WebApplication app)
    {
        app.MapGet("api/v1/Project", GetAllProjects)
            .Produces<List<Project>>()
            .RequireAuthorization("read:project")
            .WithTags("Project");

        app.MapGet("api/v1/Project/{id}", GetProjectById)
            .Produces<Project>()
            .RequireAuthorization("read:project")
            .WithTags("Project");

        app.MapPost("api/v1/Project", CreateProject)
            .Produces<ActionResult<Project>>(StatusCodes.Status201Created)
            .Produces<ActionResult<List<ValidationFailure>>>(StatusCodes.Status400BadRequest)
            .RequireAuthorization("write:project")
            .WithTags("Project");

        app.MapPut("api/v1/Project/{id}", UpdateProject)
            .Produces<IActionResult>(StatusCodes.Status404NotFound)
            .Produces<ActionResult<Project>>()
            .Produces<ActionResult<List<ValidationFailure>>>(StatusCodes.Status400BadRequest)
            .RequireAuthorization("update:project")
            .WithTags("Project");

        app.MapDelete("api/v1/Project/{id}", DeleteProject)
            .Produces<IActionResult>(StatusCodes.Status204NoContent)
            .RequireAuthorization("delete:project")
            .WithTags("Project");
    }

    internal static async Task<IResult> GetAllProjects(UnitOfWork unitOfWork, int companyId)
    {
        List<Project> projects = await unitOfWork.Projects.GetAllByCompanyIdAsync(companyId, true);
        return projects.Count == 0 ? Results.NotFound() : Results.Ok(projects);
    }

    internal static async Task<IResult> GetProjectById(int id, UnitOfWork unitOfWork)
    {
        return Results.Ok(await unitOfWork.Projects.GetByIdAsync(id, true));
    }

    internal static async Task<IResult> DeleteProject(int id, UnitOfWork unitOfWork)
    {
        Project? projectToDelete = await unitOfWork.Projects.GetByIdAsync(id, false);

        if (projectToDelete != null)
        {
            unitOfWork.BeginTransaction();
            unitOfWork.Projects.Delete(projectToDelete);
            unitOfWork.Commit();
        }

        return Results.NoContent();
    }

    internal static async Task<IResult> UpdateProject(
        int id,
        UnitOfWork unitOfWork,
        IValidator<UpdateProjectRequestModel> validator,
        UpdateProjectRequestModel req)
    {
        ValidationResult validationResult = await validator.ValidateAsync(req);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        unitOfWork.BeginTransaction();
        Project? projectToUpdate = await unitOfWork.Projects.GetByIdAsync(id, false);

        if (projectToUpdate == null)
        {
            return Results.NotFound();
        }

        projectToUpdate.UpdateName(req.Name);
        projectToUpdate.UpdatePriority(req.Priority);

        unitOfWork.Commit();
        return Results.Ok(projectToUpdate);
    }

    internal static async Task<IResult> CreateProject(
        UnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<ProjectRequestModel> validator,
        ProjectRequestModel req)
    {
        ValidationResult validationResult = await validator.ValidateAsync(req);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        unitOfWork.BeginTransaction();
        Project? project = mapper.Map<Project>(req);
        unitOfWork.Projects.Add(project);

        unitOfWork.Commit();
        return Results.Created($"api/v1/Project/{project.Id}", project);
    }
}