namespace MarketDataAggregator.Models.Positions;

public class PositionDto
{
    public required string Id { get; set; }

    public required DateTime DateTime { get; set; }

    public required string Symbol { get; set; }

    public required string Strategy { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal EntryPrice { get; set; }

    public required Direction Direction { get; set; }

    public required decimal Fee { get; set; }

    public required string FeeCurrency { get; set; }
}
