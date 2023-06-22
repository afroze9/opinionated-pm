using Nexus.Common.Abstractions.Common;

namespace Nexus.Persistence.Abstractions;

public interface ICustomRepository<T>
    where T : class, IEntity
{
    Task<List<T>> AllAsync(CancellationToken cancellationToken = default);
    
    T? GetById(int id);

    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);
}