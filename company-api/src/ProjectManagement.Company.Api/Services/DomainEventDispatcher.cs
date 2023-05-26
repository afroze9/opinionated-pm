using MediatR;
using ProjectManagement.CompanyAPI.Abstractions;
using ProjectManagement.CompanyAPI.Contracts;

namespace ProjectManagement.CompanyAPI.Services;

/// <summary>
///     Dispatcher for domain events.
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DomainEventDispatcher" /> class.
    /// </summary>
    /// <param name="mediator">The mediator.</param>
    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Dispatches and clears domain events from entities.
    /// </summary>
    /// <param name="entitiesWithEvents">The entities with domain events.</param>
    public async Task DispatchAndClearEvents(IEnumerable<EntityBase> entitiesWithEvents)
    {
        foreach (EntityBase entity in entitiesWithEvents)
        {
            DomainEventBase[] events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();

            foreach (DomainEventBase @event in events)
            {
                await _mediator.Publish(@event).ConfigureAwait(false);
            }
        }
    }
}