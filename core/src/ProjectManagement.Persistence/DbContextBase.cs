using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Persistence;

public class DbContextBase : DbContext
{
    protected DbContextBase(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}