using ProjectManagement.Core;

namespace ProjectManagement.ProjectAPI.Domain.Entities;

public class Project : AuditableEntityBase
{
    private readonly List<TodoItem> _todoItems = new ();

    public Project(string name, Priority priority, int? companyId)
    {
        Name = name;
        Priority = priority;
        CompanyId = companyId;
    }

    public int? CompanyId { get; set; }

    public string Name { get; private set; }

    public IEnumerable<TodoItem> TodoItems => _todoItems.AsReadOnly();

    public Priority Priority { get; private set; }

    public ProjectStatus Status
    {
        get
        {
            if (_todoItems.All(x => x.IsCompleted))
            {
                return ProjectStatus.Completed;
            }

            if (_todoItems.All(x => !x.IsCompleted))
            {
                return ProjectStatus.NotStarted;
            }

            return ProjectStatus.InProgress;
        }
    }

    public void AddTodoItem(TodoItem item)
    {
        _todoItems.Add(item);
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdatePriority(Priority priority)
    {
        Priority = priority;
    }
}