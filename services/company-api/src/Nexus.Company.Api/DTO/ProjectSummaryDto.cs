namespace Nexus.CompanyAPI.DTO;

[ExcludeFromCodeCoverage]
public class ProjectSummaryDto
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public string Name { get; set; }

    public int TaskCount { get; set; }
}