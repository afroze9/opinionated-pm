using Nexus.Telemetry;

namespace Nexus.Framework.Web.Configuration;

public class FrameworkSettings
{
    public AuthSettings? Auth { get; set; }

    public ApiDocumentationSettings? Swagger { get; set; }

    public FilterSettings? Filters { get; set; }
    
    public TelemetrySettings? Telemetry { get; set; }

    public ManagementSettings? Management { get; set; }

    public DiscoverySettings? Discovery { get; set; }
}

public class AuthSettings
{
    public bool Enable { get; set; }

    required public string ResourceName { get; set; }
}

public class ApiDocumentationSettings
{
    public bool Enable { get; set; }

    public string? Version { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }
}

public class FilterSettings
{
    public bool EnableActionLogging { get; set; }
}

public class ManagementSettings
{
    public bool Enable { get; set; }
}

public class DiscoverySettings
{
    public bool Enable { get; set; }
}