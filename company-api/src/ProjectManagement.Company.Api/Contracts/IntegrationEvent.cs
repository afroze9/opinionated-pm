using System.Text.Json.Serialization;

namespace ProjectManagement.CompanyAPI.Contracts;

/// <summary>
///     Base class for integration events in a distributed system.
/// </summary>
[ExcludeFromCodeCoverage]
public record IntegrationEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IntegrationEvent" /> class.
    ///     Generates a new unique identifier and sets the creation date to the current UTC date and time.
    /// </summary>
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IntegrationEvent" /> class with the specified identifier and creation
    ///     date.
    /// </summary>
    /// <param name="id">The identifier of the event.</param>
    /// <param name="createDate">The creation date of the event.</param>
    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime createDate)
    {
        Id = id;
        CreationDate = createDate;
    }

    /// <summary>
    ///     Gets the identifier of the integration event.
    /// </summary>
    [JsonInclude]
    public Guid Id { get; private init; }

    /// <summary>
    ///     Gets the creation date of the integration event in UTC.
    /// </summary>
    [JsonInclude]
    public DateTime CreationDate { get; private init; }
}