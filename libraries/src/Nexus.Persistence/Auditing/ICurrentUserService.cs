namespace Nexus.Persistence.Auditing;

public interface ICurrentUserService
{
    string? UserId { get; }
}