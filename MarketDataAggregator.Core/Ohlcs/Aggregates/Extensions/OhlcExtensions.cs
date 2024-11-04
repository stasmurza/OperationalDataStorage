using MarketDataAggregator.Models.Entities.Ohlcs;
using MarketDataAggregator.Models.Ohlcs;

namespace MarketDataAggregator.Core.Ohlcs.Aggregates.Extensions;

public static class OhlcExtensions
{
    public static void Apply(this Ohlc ohlc, OhlcDto historicalExchangeRate)
    {
        if (ohlc.Symbol != historicalExchangeRate.Symbol) throw new ArgumentOutOfRangeException(nameof(historicalExchangeRate));

        if (historicalExchangeRate.StartTime < ohlc.MinTime)
        {
            ohlc.MinTime = historicalExchangeRate.StartTime;
            ohlc.Open = historicalExchangeRate.Open;
        };

        if (historicalExchangeRate.StartTime > ohlc.MaxTime)
        {
            ohlc.MaxTime = historicalExchangeRate.StartTime;
            ohlc.Close = historicalExchangeRate.Close;
            if (historicalExchangeRate.High > ohlc.High) ohlc.High = historicalExchangeRate.High;
            if (historicalExchangeRate.Low < ohlc.Low) ohlc.Low = historicalExchangeRate.Low;
            if (historicalExchangeRate.Volume > ohlc.Volume) ohlc.Volume = historicalExchangeRate.Volume;
        }
    }

    public static OhlcDto ToHistoricalExchangeRate(this Ohlc ohlc) => new()
    {
        StartTime = ohlc.StartTime,
        Symbol = ohlc.Symbol,
        Interval = ohlc.Interval,
        Low = ohlc.Low,
        High = ohlc.High,
        Open = ohlc.Open,
        Close = ohlc.Close,
        Volume = ohlc.Volume,
    };
}
