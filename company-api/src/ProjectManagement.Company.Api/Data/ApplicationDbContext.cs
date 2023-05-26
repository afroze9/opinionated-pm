using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Contracts;
using ProjectManagement.CompanyAPI.Domain.Entities;

namespace ProjectManagement.CompanyAPI.Data;

/// <summary>
///     Application database context.
/// </summary>
[ExcludeFromCodeCoverage]
public class ApplicationDbContext : DbContext
{
    private readonly IDomainEventDispatcher? _dispatcher;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApplicationDbContext" /> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    /// <param name="dispatcher">The domain event dispatcher.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventDispatcher dispatcher)
        : base(options)
    {
        _dispatcher = dispatcher;
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

    /// <summary>
    ///     Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous save operation. The task result contains the number of state entries
    ///     written to the database.
    /// </returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new ())
    {
        int result = await base.SaveChangesAsync(cancellationToken);

        if (_dispatcher == null)
        {
            return result;
        }

        EntityBase[] entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToArray();

        await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);
        return result;
    }

    /// <summary>
    ///     Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }
}