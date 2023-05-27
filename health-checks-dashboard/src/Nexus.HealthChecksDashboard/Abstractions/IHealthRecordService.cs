using Nexus.HealthChecksDashboard.Models;

namespace Nexus.HealthChecksDashboard.Abstractions;

public interface IHealthRecordService
{
    Task<HealthRecordModel[]> GetHealthRecordsAsync(CancellationToken cancellationToken = default);
}