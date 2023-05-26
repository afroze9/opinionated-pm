namespace ProjectManagement.Persistence.Auditing;

public interface ICurrentUserService
{
    string? UserId { get; }
}