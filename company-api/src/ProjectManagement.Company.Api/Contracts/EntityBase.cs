using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.CompanyAPI.Contracts;

/// <summary>
///     Base class for all entities in the domain.
/// </summary>
public abstract class EntityBase
{
    private readonly List<DomainEventBase> _domainEvents = new ();

    /// <summary>
    ///     Gets or sets the unique identifier of the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets the collection of domain events raised by the entity.
    /// </summary>
    [NotMapped]
    public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    ///     Registers a domain event to be raised by the entity.
    /// </summary>
    /// <param name="domainEvent">The domain event to register.</param>
    protected void RegisterDomainEvent(DomainEventBase domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    ///     Clears the collection of domain events raised by the entity.
    /// </summary>
    internal void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}