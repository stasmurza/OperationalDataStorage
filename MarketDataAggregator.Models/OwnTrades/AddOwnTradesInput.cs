namespace MarketDataAggregator.Models.OwnTrades;

public class AddOwnTradesInput
{
    public required IEnumerable<OwnTradeDto> Dtos { get; set; }
}
