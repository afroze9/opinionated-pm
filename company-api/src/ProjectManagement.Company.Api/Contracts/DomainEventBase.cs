using MediatR;

namespace ProjectManagement.CompanyAPI.Contracts;

/// <summary>
///     Base class for domain events.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class DomainEventBase : INotification
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DomainEventBase" /> class.
    /// </summary>
    protected DomainEventBase()
    {
        DateOccurred = DateTime.UtcNow;
    }

    /// <summary>
    ///     Gets the date the event occurred.
    /// </summary>
    public DateTime DateOccurred { get; }
}