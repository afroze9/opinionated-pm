using ProjectManagement.Persistence;
using ProjectManagement.ProjectAPI.Data.Repositories;

namespace ProjectManagement.ProjectAPI.Data;

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