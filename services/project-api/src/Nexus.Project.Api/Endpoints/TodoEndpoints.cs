using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nexus.ProjectAPI.Data;
using Nexus.ProjectAPI.Entities;
using Nexus.ProjectAPI.Models;

namespace Nexus.ProjectAPI.Endpoints;

public static class TodoEndpoints
{
    public static void AddTodoEndpoints(this WebApplication app)
    {
        app.MapPost("api/v1/Project/{id}/Todo", AddTodoToProject)
            .RequireAuthorization("write:project")
            .WithTags("Todo");

        app.MapGet("api/v1/Todo/{id}", GetTodoById)
            .Produces<ActionResult<TodoItem>>()
            .RequireAuthorization("read:project")
            .WithTags("Todo");

        app.MapPut("api/v1/Todo/{id}", UpdateTodo)
            .Produces<ActionResult<TodoItem>>()
            .Produces<IActionResult>(StatusCodes.Status400BadRequest)
            .RequireAuthorization("update:project")
            .WithTags("Todo");
    }

    internal static async Task<IResult> UpdateTodo(int id, UnitOfWork unitOfWork,
        TodoItemAssignmentUpdateModel req)
    {
        TodoItem? itemToUpdate = unitOfWork.Todos.GetById(id);

        if (itemToUpdate == null)
        {
            return Results.BadRequest();
        }

        itemToUpdate.AssignTodoItem(req.AssignedToId);

        if (req.MarkComplete)
        {
            itemToUpdate.MarkComplete();
        }

        unitOfWork.BeginTransaction();
        unitOfWork.Todos.Update(itemToUpdate);
        unitOfWork.Commit();

        return Results.Ok(itemToUpdate);
    }

    internal static async Task<IResult> GetTodoById(int id, UnitOfWork unitOfWork)
    {
        return Results.Ok(await unitOfWork.Todos.GetByIdAsync(id));
    }

    internal static async Task<IResult> AddTodoToProject(int id, TodoItemRequestModel req,
        UnitOfWork unitOfWork, IMapper mapper)
    {
        Project? dbProject = await unitOfWork.Projects.GetByIdAsync(id, true);

        if (dbProject == null)
        {
            return Results.NotFound();
        }

        unitOfWork.BeginTransaction();
        TodoItem? todoItem = mapper.Map<TodoItem>(req);
        dbProject.AddTodoItem(todoItem);
        unitOfWork.Commit();
        
        return Results.Created($"api/v1/Todo/{todoItem.Id}", todoItem);
    }
}
