namespace MarketDataAggregator.Contracts.Ohlcs;

public class GetOhlcsRequest
{
    public required string Symbol { get; set; }
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public required TimeInterval Granularity { get; set; }
}
