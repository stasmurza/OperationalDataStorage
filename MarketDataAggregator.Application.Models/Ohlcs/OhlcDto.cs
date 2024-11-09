namespace MarketDataAggregator.Application.Models.Ohlcs;

public class OhlcDto
{
    public required string Id { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required string Symbol { get; set; }

    public TimeInterval Interval { get; set; }

    public decimal Low { get; set; }

    public decimal High { get; set; }

    public decimal Open { get; set; }

    public decimal Close { get; set; }

    public decimal Volume { get; set; }
}
