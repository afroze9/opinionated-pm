using Microsoft.EntityFrameworkCore;
using Nexus.HealthChecksDashboard.Entities;

namespace Nexus.HealthChecksDashboard.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ServiceHealthCheckRecord> HealthCheckRecords => Set<ServiceHealthCheckRecord>();

    public DbSet<InstanceHealthCheckRecord> InstanceHealthCheckRecords => Set<InstanceHealthCheckRecord>();
}