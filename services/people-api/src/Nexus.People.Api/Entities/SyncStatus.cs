using Nexus.Common;

namespace Nexus.PeopleAPI.Entities;

public class SyncStatus : AuditableNexusEntityBase
{
    public required string JobName { get; set; }
    
    public DateTime LastSync { get; set; }
}