using ProjectManagement.Telemetry;

namespace ProjectManagement.Framework.Web.Configuration;

public class FrameworkSettings
{
    public ApiDocs? Swagger { get; set; }

    public Filters? Filters { get; set; }
    
    public TelemetrySettings? Telemetry { get; set; }
}

public class ApiDocs
{
    public bool Enable { get; set; }

    public string? Version { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }
}

public class Filters
{
    public bool EnableActionLogging { get; set; }
}

