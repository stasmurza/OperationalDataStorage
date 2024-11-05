namespace MarketDataAggregator.Models.Interests;

public class GetInterestOutput
{
    public required IEnumerable<InterestDto> Interests { get; set; }
}
