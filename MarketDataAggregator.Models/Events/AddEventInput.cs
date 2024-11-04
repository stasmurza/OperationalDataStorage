using MediatR;

namespace MarketDataAggregator.Models.Events;

public class AddEventInput : IRequest
{
    public required string EventType { get; set; }

    public required string EntityType { get; set; }

    public required string EntityId { get; set; }

    public required string EventData { get; set; }
}
