using Nexus.Common.Abstractions.Common;

namespace Nexus.Common;

public abstract class AuditableEntityBase : EntityBase, IAuditable<string>
{
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = string.Empty;

    public DateTime ModifiedOn { get; set; }
}