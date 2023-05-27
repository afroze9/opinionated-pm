using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nexus.Persistence;
using Nexus.Persistence.Auditing;
using Nexus.ProjectAPI.Entities;

namespace Nexus.ProjectAPI.Data;

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