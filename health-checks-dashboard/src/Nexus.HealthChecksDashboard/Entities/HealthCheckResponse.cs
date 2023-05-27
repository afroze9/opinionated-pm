namespace Nexus.HealthChecksDashboard.Entities;

public class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;

    public LivenessDetail? Liveness { get; set; }

    public ReadinessDetail? Readiness { get; set; }

    public DatabaseDetail? Database { get; set; }

    public ConsulDetail? Consul { get; set; }

    public PingDetail? Ping { get; set; }

    public DiskSpaceDetail? DiskSpace { get; set; }
}

public class LivenessDetail
{
    public string LivenessState { get; set; } = string.Empty;
}

public class ReadinessDetail
{
    public string ReadinessState { get; set; } = string.Empty;
}

public class DatabaseDetail
{
    public string Database { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}

public class ConsulDetail
{
    public string Leader { get; set; } = string.Empty;

    public Dictionary<string, string[]> Services { get; set; } = new ();
}

public class PingDetail
{
    public int Ping { get; set; }
}

public class DiskSpaceDetail
{
    public long Total { get; set; }

    public long Free { get; set; }

    public long Threshold { get; set; }

    public string Status { get; set; } = string.Empty;
}