using ProjectManagement.CompanyAPI.Contracts;

namespace ProjectManagement.CompanyAPI.Abstractions;

/// <summary>
///     Provides a method to dispatch domain events raised by entities.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    ///     Dispatches the domain events raised by the specified entities and clears their event queues.
    /// </summary>
    /// <param name="entitiesWithEvents">An enumerable collection of entities that have raised domain events.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DispatchAndClearEvents(IEnumerable<EntityBase> entitiesWithEvents);
}