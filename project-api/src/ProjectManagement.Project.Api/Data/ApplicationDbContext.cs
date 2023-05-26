using Microsoft.EntityFrameworkCore;
using ProjectManagement.Persistence;
using ProjectManagement.Persistence.Auditing;
using ProjectManagement.ProjectAPI.Domain.Entities;

namespace ProjectManagement.ProjectAPI.Data;

[ExcludeFromCodeCoverage]
public class ApplicationDbContext : AuditableDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options, auditableEntitySaveChangesInterceptor)
    {
    }

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}