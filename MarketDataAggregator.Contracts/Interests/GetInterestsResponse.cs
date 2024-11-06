namespace MarketDataAggregator.Contracts.Interests;

public class GetInterestsResponse
{
    public required IEnumerable<InterestDto> Interests { get; set; }
}
