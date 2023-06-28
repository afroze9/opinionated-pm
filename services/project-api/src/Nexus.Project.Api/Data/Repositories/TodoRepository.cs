using Nexus.Persistence;
using Nexus.ProjectAPI.Entities;

namespace Nexus.ProjectAPI.Data.Repositories;

public class TodoRepository : EfNexusRepository<TodoItem>
{
    public TodoRepository(ApplicationDbContext context) : base(context)
    {
    }
}