namespace MarketDataAggregator.Application.Models.Interests;

public class GetInterestsOutput
{
    public required IEnumerable<InterestDto> Interests { get; set; }
}
