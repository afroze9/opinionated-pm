namespace ProjectManagement.CompanyAPI.Contracts;

/// <summary>
///     Represents an integration event for when a tag is deleted from a company.
/// </summary>
[ExcludeFromCodeCoverage]
public record CompanyTagDeletedIntegrationEvent(int CompanyId, string TagName) : IntegrationEvent;