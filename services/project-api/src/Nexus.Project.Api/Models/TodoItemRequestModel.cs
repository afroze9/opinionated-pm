namespace Nexus.ProjectAPI.Models;

[ExcludeFromCodeCoverage]
public record TodoItemRequestModel(string Title, string? Description, string? AssignedToId);