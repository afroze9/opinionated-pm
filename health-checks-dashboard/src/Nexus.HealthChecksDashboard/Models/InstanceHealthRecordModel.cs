using Nexus.HealthChecksDashboard.Entities;

namespace Nexus.HealthChecksDashboard.Models;

public class InstanceHealthRecordModel
{
    public DateTime CreatedAt { get; set; }
    
    public int InstanceNumber { get; set; }
    
    public HealthCheckResponse? Response { get; set; }
}