using MarketDataAggregator.Core.Ohlcs.Extensions;
using MarketDataAggregator.Entities.Ohlcs;
using MarketDataAggregator.Models.Ohlcs;

namespace MarketDataAggregator.Core.Ohlcs;

public class OhlcAggregator()
{
    public static IEnumerable<Ohlc> Process(IEnumerable<OhlcDto> dtos)
    {
        if (!dtos.Any()) return Enumerable.Empty<Ohlc>();

        var dictionary = dtos
            .GroupBy(i => new
            {
                i.StartTime,
                i.Symbol
            })
            .ToDictionary(i => i.Key, i => i.ToList());

        var intervals = new List<Entities.Ohlcs.TimeInterval>()
        {
            Entities.Ohlcs.TimeInterval.Days1,
            Entities.Ohlcs.TimeInterval.Hours1,
            Entities.Ohlcs.TimeInterval.Minutes1
        };

        var aggregates = new Dictionary<string, Ohlc>();
        foreach (var keyValuePair in dictionary)
        {
            var exchangeRates = keyValuePair.Value;
            if (exchangeRates.Count == 0) continue;
            UpdateAggregates(aggregates, keyValuePair.Key.StartTime, keyValuePair.Key.Symbol, intervals, exchangeRates);
        }

        return aggregates.Values;
    }

    private static void UpdateAggregates(Dictionary<string, Ohlc> aggregates, DateTime startTime, string symbol, IEnumerable<Entities.Ohlcs.TimeInterval> timeIntervals, IEnumerable<OhlcDto> dtos)
    {
        foreach (var timeInterval in timeIntervals)
        {
            var applicableDtos = dtos.Where(i => (int)i.Interval <= (int)timeInterval);
            if (!applicableDtos.Any()) continue;
            
            var key = symbol + startTime.ToString() + timeInterval;
            foreach (var dto in applicableDtos)
            {
                if (!aggregates.TryGetValue(key, out var aggregate)) aggregate = Create(dto, startTime, timeInterval);
                else aggregate.Apply(dto);
            }
        }
    }

    private static Ohlc Create(OhlcDto dto, DateTime startTime, Entities.Ohlcs.TimeInterval interval) => new()
    {
        Symbol = dto.Symbol,
        Interval = interval,
        StartTime = startTime,
        EndTime = dto.EndTime,
        Low = dto.Low,
        High = dto.High,
        Open = dto.Open,
        Close = dto.Close,
        Volume = dto.Volume,
    };
}
