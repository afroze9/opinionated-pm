using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nexus.PeopleAPI.Entities;
using Nexus.Persistence;
using Nexus.Persistence.Auditing;

namespace Nexus.PeopleAPI.Data;

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
    
    public DbSet<Person> People => Set<Person>();
}