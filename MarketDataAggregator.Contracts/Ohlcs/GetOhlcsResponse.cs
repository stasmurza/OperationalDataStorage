namespace MarketDataAggregator.Contracts.Ohlcs;

public struct GetOhlcsResponse
{
    public IEnumerable<OhlcDto> Ohlcs { get; set; }
}
