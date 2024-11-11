namespace OperationalDataStorage.Contracts.Events;

/// <summary>
/// Snapshot of new events.
/// </summary>
public class EventsSnapshot
{
    /// <summary>
    /// Events.
    /// </summary>
    public required IEnumerable<EventDto> NewEvents { get; set; } = [];
}
