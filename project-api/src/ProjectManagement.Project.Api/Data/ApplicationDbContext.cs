using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Persistence;
using ProjectManagement.Persistence.Auditing;
using ProjectManagement.ProjectAPI.Entities;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}