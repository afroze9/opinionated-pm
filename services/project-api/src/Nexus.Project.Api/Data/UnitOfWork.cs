using Nexus.Common.Attributes;
using Nexus.Persistence;
using Nexus.ProjectAPI.Data.Repositories;

namespace Nexus.ProjectAPI.Data;

[NexusService(NexusServiceLifeTime.Scoped)]
public class UnitOfWork : UnitOfWorkBase
{
    public UnitOfWork(ApplicationDbContext context, ProjectRepository projectRepository,
        TodoRepository todoRepository) : base(context)
    {
        Projects = projectRepository;
        Todos = todoRepository;
    }

    public ProjectRepository Projects { get; }

    public TodoRepository Todos { get; }
}