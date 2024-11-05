using MarketDataAggregator.Entities.Abstractions;

namespace MarketDataAggregator.Entities.Positions;

public class Position : IEntity
{
    public string Id { get; set; } = string.Empty;

    public required DateTime DateTime { get; set; }

    public required string Symbol { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal EntryPrice { get; set; }

    public required Direction Direction { get; set; }

    public required decimal Fee { get; set; }

    public required string FeeCurrency { get; set; }
}
