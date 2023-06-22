using Nexus.Common;

namespace Nexus.ProjectAPI.Entities;

public class TodoItem : AuditableEntityBase
{
    required public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public string? AssignedToId { get; set; }

    public bool IsCompleted { get; private set; }

    public void MarkComplete()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
        }
    }

    public void AssignTodoItem(string assignedToId)
    {
        AssignedToId = assignedToId;
    }
}