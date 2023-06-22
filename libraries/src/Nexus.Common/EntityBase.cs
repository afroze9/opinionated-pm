using Nexus.Common.Abstractions.Common;

namespace Nexus.Common;

public abstract class EntityBase : IEntity
{
    public int Id { get; set; }
}