namespace ProjectManagement.Framework.Web.Configuration;

public class FrameworkSettings
{
    public SwaggerSettings? Swagger { get; set; }

    public FilterSettings? Filters { get; set; }
}

public class SwaggerSettings
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