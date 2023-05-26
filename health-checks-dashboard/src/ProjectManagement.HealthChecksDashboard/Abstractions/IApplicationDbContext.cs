using Microsoft.EntityFrameworkCore;
using ProjectManagement.HealthChecksDashboard.Entities;

namespace ProjectManagement.HealthChecksDashboard.Abstractions;

public interface IApplicationDbContext
{
    DbSet<HealthCheckRecord> HealthCheckRecords { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}