using MarketDataAggregator.Entities.Abstractions;

namespace MarketDataAggregator.Entities.Positions;

public class Position : IEntity
{
    public string Id { get; set; } = string.Empty;

    public required string Symbol { get; set; }

    public required string Strategy { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal EntryPrice { get; set; }

    public required Direction Direction { get; set; }
}
