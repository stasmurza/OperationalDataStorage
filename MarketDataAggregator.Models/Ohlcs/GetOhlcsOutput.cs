namespace MarketDataAggregator.Models.Ohlcs;

public struct GetOhlcsOutput
{
    public IEnumerable<OhlcDto> HistoricalExchangeRates { get; set; }
}
