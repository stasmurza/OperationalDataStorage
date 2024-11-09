using MediatR;

namespace MarketDataAggregator.Application.Models.Ohlcs;

public struct AddOhlcInput : IRequest
{
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
