namespace ProjectManagement.HealthChecksDashboard.Abstractions;

public interface IHealthChecksService
{
    Task CheckHealthAsync();
}