namespace MarketDataAggregator.Contracts.Events;

public class EventDto
{
    public required string Id { get; set; }

    public required DateTime DateTime { get; set; }

    public required string EventType { get; set; }

    public required string EntityType { get; set; } = string.Empty;

    public required string EntityId { get; set; } = string.Empty;

    public required string EventData { get; set; } = string.Empty;
}
