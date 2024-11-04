using MarketDataAggregator.Core.Ohlcs.Aggregates.Extensions;
using MarketDataAggregator.Models.Ohlcs;

namespace MarketDataAggregator.Core.Ohlcs.Converters;

public static class ModelConverter
{
    public static OhlcDto Convert(AddOhlcInput addHistoricalDataInput)
    {
        return new OhlcDto
        {
            StartTime = addHistoricalDataInput.EndTime.ToStartTime(addHistoricalDataInput.Interval),
            Symbol = addHistoricalDataInput.Symbol,
            Interval = addHistoricalDataInput.Interval,
            Low = addHistoricalDataInput.Low,
            High = addHistoricalDataInput.High,
            Open = addHistoricalDataInput.Open,
            Close = addHistoricalDataInput.Close,
            Volume = addHistoricalDataInput.Volume,
        };
    }
}
