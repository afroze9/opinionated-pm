using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nexus.CompanyAPI.Entities;
using Nexus.Persistence;
using Nexus.Persistence.Auditing;

namespace Nexus.CompanyAPI.Data;

/// <summary>
///     Application database context.
/// </summary>
[ExcludeFromCodeCoverage]
public class ApplicationDbContext : AuditableDbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApplicationDbContext" /> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    /// <param name="auditableEntitySaveChangesInterceptor">Service to handle audit information.</param>
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
        modelBuilder.Entity<Company>().HasData(_companies);
        modelBuilder.Entity<Tag>().HasData(_tags);
        modelBuilder.Entity("CompanyTag").HasData(_companyTags);
    }
    
    /// <summary>
    ///     Gets the companies.
    /// </summary>
    public DbSet<Company> Companies => Set<Company>();

    /// <summary>
    ///     Gets the tags.
    /// </summary>
    public DbSet<Tag> Tags => Set<Tag>();
    
    private List<Company> _companies = new ()
    {
        new Company("Undev9")
        {
            Id = 1,
        },
        new Company("Nexus Inc.")
        {
            Id = 2,
        },
    };

    private List<Tag> _tags = new ()
    {
        new Tag("software") { Id = 1 },
        new Tag("development") { Id = 2 },
    };


    private dynamic _companyTags = new[]
    {
        new { CompanyId = 1, TagId = 1 },
        new { CompanyId = 1, TagId = 2 },
        new { CompanyId = 2, TagId = 1 },
        new { CompanyId = 2, TagId = 2 },
    };
}