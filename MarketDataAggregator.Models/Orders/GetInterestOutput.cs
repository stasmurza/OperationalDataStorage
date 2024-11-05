namespace MarketDataAggregator.Models.Orders;

public class GetInterestOutput
{
    public required string Strategy { get; set; }

    public required IEnumerable<InterestDto> Interests { get; set; }
}
