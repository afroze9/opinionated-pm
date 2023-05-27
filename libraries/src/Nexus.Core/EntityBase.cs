using Nexus.Core.Abstractions.Common;

namespace Nexus.Core;

public abstract class EntityBase : IEntity
{
    public int Id { get; set; }
}