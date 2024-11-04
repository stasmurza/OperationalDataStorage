namespace MarketDataAggregator.Contracts.Ohlcs;

public struct GetOhlcsResponse
{
    public IEnumerable<OhlcDto> HistoricalExchangeRates { get; set; }
}
