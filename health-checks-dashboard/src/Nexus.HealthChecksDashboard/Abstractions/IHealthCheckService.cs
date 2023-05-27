namespace Nexus.HealthChecksDashboard.Abstractions;

public interface IHealthChecksService
{
    Task CheckHealthAsync();
}