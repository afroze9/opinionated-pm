namespace Nexus.Persistence.Abstractions;

public interface IUnitOfWork : IDisposable
{
    void Commit();

    void Rollback();

    void BeginTransaction();
}