using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Nexus.ProjectAPI.Data;
using Nexus.ProjectAPI.Entities;
using Nexus.ProjectAPI.Models;
using Nexus.SharedKernel.Contracts.Project;
// ReSharper disable MemberCanBePrivate.Global

namespace Nexus.ProjectAPI.Endpoints;

public static class ProjectEndpoints
{
    public static void AddProjectEndpoints(this WebApplication app)
    {
        app.MapGet("api/v1/Project", GetAllProjects)
            .Produces<List<ProjectResponseModel>>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization("read:project")
            .WithTags("Project");

        app.MapGet("api/v1/Project/{id}", GetProjectById)
            .Produces<ProjectResponseModel>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization("read:project")
            .WithTags("Project");

        app.MapPost("api/v1/Project", CreateProject)
            .Produces<ActionResult<ProjectResponseModel>>(StatusCodes.Status201Created)
            .Produces<ActionResult<List<ValidationFailure>>>(StatusCodes.Status400BadRequest)
            .RequireAuthorization("write:project")
            .WithTags("Project");

        app.MapPut("api/v1/Project/{id}", UpdateProject)
            .Produces<ActionResult<ProjectResponseModel>>()
            .Produces<IActionResult>(StatusCodes.Status404NotFound)
            .Produces<ActionResult<List<ValidationFailure>>>(StatusCodes.Status400BadRequest)
            .RequireAuthorization("update:project")
            .WithTags("Project");

        app.MapDelete("api/v1/Project/{id}", DeleteProject)
            .Produces<IActionResult>(StatusCodes.Status204NoContent)
            .RequireAuthorization("delete:project")
            .WithTags("Project");
    }

    internal static async Task<IResult> GetAllProjects(UnitOfWork unitOfWork, IMapper mapper, int? companyId)
    {
        List<Project> projects = await unitOfWork.Projects.GetAllByCompanyIdAsync(companyId, true);
        return projects.Count == 0 ? Results.NotFound() : Results.Ok(mapper.Map<List<ProjectResponseModel>>(projects));
    }

    internal static async Task<IResult> GetProjectById(int id, UnitOfWork unitOfWork, IMapper mapper)
    {
        Project? project = await unitOfWork.Projects.GetByIdAsync(id, true);
        return project == null ? Results.NotFound() : Results.Ok(mapper.Map<ProjectResponseModel>(project));
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
        UpdateProjectRequestModel req,
        IMapper mapper)
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
        return Results.Ok(mapper.Map<ProjectResponseModel>(projectToUpdate));
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
        return Results.Created($"api/v1/Project/{project.Id}", mapper.Map<ProjectResponseModel>(project));
    }
}