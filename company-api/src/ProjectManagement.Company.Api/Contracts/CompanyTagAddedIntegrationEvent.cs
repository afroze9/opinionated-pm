namespace ProjectManagement.CompanyAPI.Contracts;

/// <summary>
///     Represents an integration event for when a tag is added to a company.
/// </summary>
[ExcludeFromCodeCoverage]
public record CompanyTagAddedIntegrationEvent(int CompanyId, string TagName) : IntegrationEvent;