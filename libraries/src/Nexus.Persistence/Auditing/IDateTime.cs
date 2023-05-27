namespace Nexus.Persistence.Auditing;

public interface IDateTime
{
    DateTime Now { get; }

    DateTime UtcNow { get; }
}