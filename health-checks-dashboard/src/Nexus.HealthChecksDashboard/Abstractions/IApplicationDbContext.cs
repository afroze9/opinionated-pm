using Microsoft.EntityFrameworkCore;
using Nexus.HealthChecksDashboard.Entities;

namespace Nexus.HealthChecksDashboard.Abstractions;

public interface IApplicationDbContext
{
    DbSet<HealthCheckRecord> HealthCheckRecords { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}