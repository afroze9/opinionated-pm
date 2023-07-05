namespace Nexus.SharedKernel.Contracts.Project;

[ExcludeFromCodeCoverage]
public class ProjectSummaryResponseModel
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int TaskCount { get; set; }
}