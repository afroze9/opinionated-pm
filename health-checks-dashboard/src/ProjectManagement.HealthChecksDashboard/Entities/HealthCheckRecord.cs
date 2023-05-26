namespace ProjectManagement.HealthChecksDashboard.Entities;

public class HealthCheckRecord
{
    public int Id { get; set; }

    required public string ClientName { get; set; }

    required public string Response { get; set; }

    public DateTime CreatedAt { get; set; }
}