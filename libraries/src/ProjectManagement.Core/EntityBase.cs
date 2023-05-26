using ProjectManagement.Core.Abstractions.Common;

namespace ProjectManagement.Core;

public abstract class EntityBase : IEntity
{
    public int Id { get; set; }
}