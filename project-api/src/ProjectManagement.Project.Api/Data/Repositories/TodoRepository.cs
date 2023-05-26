using Microsoft.EntityFrameworkCore;
using ProjectManagement.Persistence;
using ProjectManagement.ProjectAPI.Domain.Entities;

namespace ProjectManagement.ProjectAPI.Data.Repositories;

public class TodoRepository : EfCustomRepository<TodoItem>
{
    protected TodoRepository(DbContext context) : base(context)
    {
    }
}