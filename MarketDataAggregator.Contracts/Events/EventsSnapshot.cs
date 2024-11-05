namespace MarketDataAggregator.Contracts.Events;

public class EventsSnapshot
{
    public required IEnumerable<EventDto> Events { get; set; } = [];
}
