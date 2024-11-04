namespace MarketDataAggregator.Contracts.Ohlcs;

public class AddOhlcRequest
{
    public required DateTime EndTime { get; set; }

    public required string Symbol { get; set; }

    public required TimeInterval Interval { get; set; }

    public required decimal Low { get; set; }

    public required decimal High { get; set; }

    public required decimal Open { get; set; }

    public required decimal Close { get; set; }

    public required decimal Volume { get; set; }
}
