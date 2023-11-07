namespace Nexus.HealthChecksDashboard.Models;

public class ServiceHealthRecordModel
{
    public string ClientName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public List<InstanceHealthRecordModel> InstanceHealthRecords { get; set; } = new ();
    public bool Expanded { get; set; }

    public string Status
    {
        get
        {
            if (InstanceHealthRecords.Count == 0)
            {
                return "DOWN";
            }

            if (InstanceHealthRecords.All(x => x.Response?.Status.ToUpper() == "UP"))
            {
                return "UP";
            }
            
            if (InstanceHealthRecords.All(x => x.Response?.Status.ToUpper() == "DOWN"))
            {
                return "DOWN";
            }

            return "UP (P)";
        }
    }
        

    public string Database
    {
        get
        {
            if (InstanceHealthRecords.Count == 0)
            {
                return "DOWN";
            }

            if (InstanceHealthRecords.All(x => x.Response?.Database?.Status.ToUpper() == "UP"))
            {
                return "UP";
            }
            
            if (InstanceHealthRecords.All(x => x.Response?.Database?.Status.ToUpper() == "DOWN"))
            {
                return "DOWN";
            }

            return "UP (P)";
        }
    }

    public string Liveness
    {
        get
        {
            if (InstanceHealthRecords.Count == 0)
            {
                return "BROKEN";
            }

            if (InstanceHealthRecords.All(x => x.Response?.Liveness?.LivenessState.ToUpper() == "CORRECT"))
            {
                return "CORRECT";
            }
            
            if (InstanceHealthRecords.All(x => x.Response?.Liveness?.LivenessState.ToUpper() == "BROKEN"))
            {
                return "BROKEN";
            }

            return "CORRECT (P)";
        }
    }

    public string Readiness
    {
        get
        {
            if (InstanceHealthRecords.Count == 0)
            {
                return "REFUSING_TRAFFIC";
            }

            if (InstanceHealthRecords.All(x => x.Response?.Readiness?.ReadinessState.ToUpper() == "ACCEPTING_TRAFFIC"))
            {
                return "ACCEPTING_TRAFFIC";
            }
            
            if (InstanceHealthRecords.All(x => x.Response?.Readiness?.ReadinessState.ToUpper() == "REFUSING_TRAFFIC"))
            {
                return "REFUSING_TRAFFIC";
            }

            return "ACCEPTING_TRAFFIC (P)";
        }
    }
}