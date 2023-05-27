using Microsoft.EntityFrameworkCore;
using Nexus.HealthChecksDashboard.Abstractions;
using Nexus.HealthChecksDashboard.Entities;

namespace Nexus.HealthChecksDashboard.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<HealthCheckRecord> HealthCheckRecords => Set<HealthCheckRecord>();
}