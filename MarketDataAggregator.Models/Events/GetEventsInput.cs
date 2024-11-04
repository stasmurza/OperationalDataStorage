using MarketDataAggregator.Models.Entities.Events;
using MarketDataAggregator.Models.Ohlcs;
using MediatR;

namespace MarketDataAggregator.Models.Events;

public class GetEventsInput : IRequest<GetEventsOutput>
{
    public required DateTime Start { get; set; }

    public required DateTime End { get; set; }

    public EventType? EventType { get; set; }

    public EntityType? EntityType { get; set; }
}
