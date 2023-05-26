using Ardalis.Specification;
using ProjectManagement.Core.Abstractions.Common;

namespace ProjectManagement.Persistence.Abstractions;

/// <summary>
///     Provides a repository for entities of type T that implement the IAggregateRoot interface.
/// </summary>
/// <typeparam name="T">The type of entity to be retrieved or modified.</typeparam>
public interface IRepository<T> : IRepositoryBase<T>
    where T : class, IEntity
{
    // This interface inherits all of its member documentation from the IRepositoryBase<T> interface.
}
