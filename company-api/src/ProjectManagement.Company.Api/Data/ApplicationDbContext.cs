using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.CompanyAPI.Domain.Entities;

namespace ProjectManagement.CompanyAPI.Data;

/// <summary>
///     Application database context.
/// </summary>
[ExcludeFromCodeCoverage]
public class ApplicationDbContext : DbContext
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApplicationDbContext" /> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    ///     Gets the companies.
    /// </summary>
    public DbSet<Company> Companies => Set<Company>();

    /// <summary>
    ///     Gets the tags.
    /// </summary>
    public DbSet<Tag> Tags => Set<Tag>();

    /// <summary>
    ///     Configures the model that was discovered by convention from the entity types
    ///     exposed in <see cref="DbSet{TEntity}" /> properties on your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}