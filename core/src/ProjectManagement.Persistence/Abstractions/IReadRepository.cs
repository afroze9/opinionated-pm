using Ardalis.Specification;
using ProjectManagement.Core.Abstractions.Common;

namespace ProjectManagement.Persistence.Abstractions;

/// <summary>
///     Provides a read-only repository for entities of type T that implement the IAggregateRoot interface.
/// </summary>
/// <typeparam name="T">The type of entity to be retrieved.</typeparam>
public interface IReadRepository<T> : IReadRepositoryBase<T>
    where T : class, IEntity
{
    // This interface inherits all of its member documentation from the IReadRepositoryBase<T> interface.
}