using Nexus.Persistence;
using Nexus.ProjectAPI.Entities;

namespace Nexus.ProjectAPI.Data.Repositories;

public class TodoRepository : EfCustomRepository<TodoItem>
{
    public TodoRepository(ApplicationDbContext context) : base(context)
    {
    }
}