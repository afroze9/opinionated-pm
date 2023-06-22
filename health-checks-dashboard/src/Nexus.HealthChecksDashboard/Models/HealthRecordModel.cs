using Nexus.HealthChecksDashboard.Entities;

namespace Nexus.HealthChecksDashboard.Models;

public class HealthRecordModel
{
    public string ClientName { get; set; } = string.Empty;

    public HealthCheckResponse? Response { get; set; }

    public DateTime CreatedAt { get; set; }
}