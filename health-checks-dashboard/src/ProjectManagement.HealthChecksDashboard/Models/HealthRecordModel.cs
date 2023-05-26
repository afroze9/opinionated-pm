using ProjectManagement.HealthChecksDashboard.Entities;

namespace ProjectManagement.HealthChecksDashboard.Models;

public class HealthRecordModel
{
    public string ClientName { get; set; }

    public HealthCheckResponse? Response { get; set; }

    public DateTime CreatedAt { get; set; }
}