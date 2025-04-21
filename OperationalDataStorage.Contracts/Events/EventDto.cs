namespace OperationalDataStorage.Contracts.Events;

/// <summary>
/// Event DTO.
/// </summary>
public class EventDto
{
    /// <summary>
    /// Id.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Event identifier.
    /// </summary>
    public required Guid EventId { get; set; }

    /// <summary>
    /// Date of the event.
    /// </summary>
    public required DateTime EventDateTime { get; set; }

    /// <summary>
    /// Event type.
    /// </summary>
    public required EventType EventType { get; set; }

    /// <summary>
    /// Event data.
    /// </summary>
    public required string EventData { get; set; }

    /// <summary>
    /// Entity type.
    /// </summary>
    public required EntityType EntityType { get; set; }

    /// <summary>
    /// Entity id.
    /// </summary>
    public required string EntityId { get; set; }
}
