using MediatR;

namespace MarketDataAggregator.Models.Ohlcs;

public struct GetOhlcsInput : IRequest<GetOhlcsOutput>
{
    public required string Symbol { get; set; }
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public TimeInterval Granularity { get; set; }
}
