using Microsoft.EntityFrameworkCore;
using ProjectManagement.HealthChecksDashboard.Abstractions;
using ProjectManagement.HealthChecksDashboard.Entities;

namespace ProjectManagement.HealthChecksDashboard.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<HealthCheckRecord> HealthCheckRecords => Set<HealthCheckRecord>();
}