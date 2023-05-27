using ProjectManagement.Persistence;
using ProjectManagement.ProjectAPI.Entities;

namespace ProjectManagement.ProjectAPI.Data.Repositories;

public class TodoRepository : EfCustomRepository<TodoItem>
{
    public TodoRepository(ApplicationDbContext context) : base(context)
    {
    }
}