namespace Nexus.HealthChecksDashboard.Entities;

public class ServiceHealthCheckRecord
{
    public int Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    required public string ClientName { get; set; }
    public List<InstanceHealthCheckRecord> InstanceRecords { get; set; } = new ();
}