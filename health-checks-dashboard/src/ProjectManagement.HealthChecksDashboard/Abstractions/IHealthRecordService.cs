using ProjectManagement.HealthChecksDashboard.Models;

namespace ProjectManagement.HealthChecksDashboard.Abstractions;

public interface IHealthRecordService
{
    Task<HealthRecordModel[]> GetHealthRecordsAsync(CancellationToken cancellationToken = default);
}