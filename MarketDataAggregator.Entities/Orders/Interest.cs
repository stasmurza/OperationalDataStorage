using MarketDataAggregator.Entities.Abstractions;

namespace MarketDataAggregator.Entities.Orders;

public class Interest : IEntity
{
    public string Id { get; set; } = string.Empty;

    public required string Symbol { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal AveragePrice { get; set; }

    public required Direction Direction { get; set; }
}
