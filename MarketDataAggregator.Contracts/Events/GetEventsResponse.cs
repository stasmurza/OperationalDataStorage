namespace MarketDataAggregator.Contracts.Events;

public class GetEventsResponse
{
    public required IEnumerable<EventDto> Events { get; set; }
}
