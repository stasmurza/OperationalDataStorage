namespace MarketDataAggregator.Contracts;

public class MarketDataEventArgs
{
    public required string Message { get; set; }

    public required string RoutingKey { get; set; }
}
