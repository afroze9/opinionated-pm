namespace Nexus.HealthChecksDashboard.Entities;

public class InstanceHealthCheckRecord
{
    public int Id { get; set; }
    
    public int InstanceNumber { get; set; }

    public DateTime CreatedAt { get; set; }
    
    required public string Response { get; set; }    
}