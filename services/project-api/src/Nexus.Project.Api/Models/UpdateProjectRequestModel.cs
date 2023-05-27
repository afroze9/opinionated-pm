using Nexus.ProjectAPI.Entities;

namespace Nexus.ProjectAPI.Models;

[ExcludeFromCodeCoverage]
public record UpdateProjectRequestModel(int Id, string Name, int? CompanyId, Priority Priority = Priority.Medium);