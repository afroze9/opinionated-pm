using Nexus.Common;

namespace Nexus.ProjectAPI.Entities;

public class TodoItem : AuditableNexusEntityBase
{
    public required string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public string? AssignedToId { get; set; }

    public bool IsCompleted { get; private set; }
    
    public int ProjectId { get; set; }

    public void MarkComplete(bool isCompleted)
    {
        IsCompleted = isCompleted;
    }

    public void AssignTodoItem(string assignedToId)
    {
        AssignedToId = assignedToId;
    }
}