using MarketDataAggregator.Models.Entities.Ohlcs;

namespace MarketDataAggregator.Models.Ohlcs.Aggregates;

public class AggregateUpdatedEventArgs
{
    public required Ohlc Ohlc { get; set; }
}
