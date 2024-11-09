namespace MarketDataAggregator.Application.Models.Positions;

public class PositionDto
{
    public required string Id { get; set; }

    public required string Symbol { get; set; }

    public required string Strategy { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal EntryPrice { get; set; }

    public required Direction Direction { get; set; }

    public required List<OwnTradeDto> OwnTrades { get; set; }
}
