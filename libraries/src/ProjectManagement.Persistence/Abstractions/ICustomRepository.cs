using ProjectManagement.Core.Abstractions.Common;

namespace ProjectManagement.Persistence.Abstractions;

public interface ICustomRepository<T>
    where T : class, IEntity
{
    T? GetById(int id);

    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);
}