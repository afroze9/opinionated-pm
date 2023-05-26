using Ardalis.Specification;

namespace ProjectManagement.CompanyAPI.Abstractions;

/// <summary>
///     Provides a read-only repository for entities of type T that implement the IAggregateRoot interface.
/// </summary>
/// <typeparam name="T">The type of entity to be retrieved.</typeparam>
public interface IReadRepository<T> : IReadRepositoryBase<T>
    where T : class, IAggregateRoot
{
    // This interface inherits all of its member documentation from the IReadRepositoryBase<T> interface.
}