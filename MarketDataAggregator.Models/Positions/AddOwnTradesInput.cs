namespace MarketDataAggregator.Models.Positions;

public class AddOwnTradesInput
{
    public required IEnumerable<OwnTradeDto> Dtos { get; set; }
}
