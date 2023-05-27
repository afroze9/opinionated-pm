using Nexus.ProjectAPI.Entities;

namespace Nexus.ProjectAPI.Models;

[ExcludeFromCodeCoverage]
public record ProjectRequestModel(string Name, int? CompanyId, Priority Priority = Priority.Medium);