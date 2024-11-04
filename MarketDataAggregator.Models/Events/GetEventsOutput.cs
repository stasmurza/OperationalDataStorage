namespace MarketDataAggregator.Models.Events;

public class GetEventsOutput
{
    public required IEnumerable<EventDto> Events { get; set; }
}
