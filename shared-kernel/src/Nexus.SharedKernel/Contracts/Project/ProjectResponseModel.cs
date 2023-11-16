namespace Nexus.SharedKernel.Contracts.Project;

[ExcludeFromCodeCoverage]
public class ProjectResponseModel
{
    public int Id { get; set; }

    public int? CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ProjectPriority Priority { get; private set; }

    public List<TodoItemResponseModel> TodoItems { get; set; } = new ();
}

public enum ProjectPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4,
}
