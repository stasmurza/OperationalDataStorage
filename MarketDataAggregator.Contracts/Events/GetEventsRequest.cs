namespace MarketDataAggregator.Contracts.Events;

public class GetEventsRequest
{
    public required DateTime Start { get; set; }

    public required DateTime End { get; set; }

    public required IEnumerable<EventType> EventTypes { get; set; }

    public required IEnumerable<EntityType> EntityTypes { get; set; }
}
