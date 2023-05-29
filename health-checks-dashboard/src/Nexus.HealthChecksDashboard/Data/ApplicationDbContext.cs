using Microsoft.EntityFrameworkCore;
using Nexus.HealthChecksDashboard.Entities;

namespace Nexus.HealthChecksDashboard.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<HealthCheckRecord> HealthCheckRecords => Set<HealthCheckRecord>();
}