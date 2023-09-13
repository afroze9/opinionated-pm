namespace Nexus.ProjectAPI.Models;

[ExcludeFromCodeCoverage]
public record TodoItemAssignmentUpdateModel(bool IsCompleted, string? AssignedToId);