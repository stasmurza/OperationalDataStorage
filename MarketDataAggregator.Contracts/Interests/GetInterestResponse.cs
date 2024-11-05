namespace MarketDataAggregator.Contracts.Interests;

public class GetInterestResponse
{
    public required IEnumerable<InterestDto> Positions { get; set; }
}
