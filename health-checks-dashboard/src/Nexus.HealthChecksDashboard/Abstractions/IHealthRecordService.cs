using Nexus.HealthChecksDashboard.Models;

namespace Nexus.HealthChecksDashboard.Abstractions;

public interface IHealthRecordService
{
    Task<ServiceHealthRecordModel[]> GetHealthRecordsAsync(CancellationToken cancellationToken = default);
}