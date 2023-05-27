using Nexus.HealthChecksDashboard.Entities;

namespace Nexus.HealthChecksDashboard.Models;

public class HealthRecordModel
{
    public string ClientName { get; set; }

    public HealthCheckResponse? Response { get; set; }

    public DateTime CreatedAt { get; set; }
}