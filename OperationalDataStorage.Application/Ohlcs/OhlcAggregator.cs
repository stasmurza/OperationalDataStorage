using OperationalDataStorage.Application.Ohlcs.Extensions;
using OperationalDataStorage.Application.Models.Ohlcs;
using OperationalDataStorage.Domain.Entities.Ohlcs;

namespace OperationalDataStorage.Application.Ohlcs;

public class OhlcAggregator()
{
    public static IEnumerable<Ohlc> Process(IEnumerable<OhlcDto> dtos)
    {
        if (!dtos.Any()) return [];

        var timeIntervals = new List<Domain.Entities.Ohlcs.TimeInterval>()
        {
            Domain.Entities.Ohlcs.TimeInterval.Days1,
            Domain.Entities.Ohlcs.TimeInterval.Hours1,
            Domain.Entities.Ohlcs.TimeInterval.Minutes1
        };

        var min1Dtos = dtos.Where(i => i.Interval == Models.Ohlcs.TimeInterval.Minutes1);
        var aggregates = new Dictionary<string, Ohlc>();
        foreach (var dto in dtos)
        {
            var intervalStart = GetStartDateTime(dto.StartTime, dto.Interval);
            var key = dto.Interval.ToString() + intervalStart.ToString() + dto.Symbol;
            if (!aggregates.TryGetValue(key, out var aggregate))
            {
                aggregate = Create(dto);
                aggregates.Add(key, aggregate);
            }
            else aggregate.Apply(dto);
        }

        return aggregates.Values;
    }

    private static Ohlc Create(OhlcDto dto) => new()
    {
        Symbol = dto.Symbol,
        Interval = Enum.Parse<Domain.Entities.Ohlcs.TimeInterval>(dto.Interval.ToString()),
        StartTime = dto.StartTime,
        EndTime = dto.EndTime,
        Low = dto.Low,
        High = dto.High,
        Open = dto.Open,
        Close = dto.Close,
        Volume = dto.Volume,
    };

    private static DateTime GetStartDateTime(DateTime startTime, Models.Ohlcs.TimeInterval timeInterval) => timeInterval switch
    {
        Models.Ohlcs.TimeInterval.Days1 => startTime.Date,
        Models.Ohlcs.TimeInterval.Hours1 => new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0),
        Models.Ohlcs.TimeInterval.Minutes1 => new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(timeInterval), $"Not expected direction value: {timeInterval}"),
    };
    
    private static DateTime GetEndDateTime(DateTime startTime, Models.Ohlcs.TimeInterval timeInterval) => timeInterval switch
    {
        Models.Ohlcs.TimeInterval.Days1 => startTime.Date.AddDays(1).AddTicks(-1),
        Models.Ohlcs.TimeInterval.Hours1 => new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0).AddHours(1).AddTicks(-1),
        Models.Ohlcs.TimeInterval.Minutes1 => new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0).AddMinutes(1).AddTicks(-1),
        _ => throw new ArgumentOutOfRangeException(nameof(timeInterval), $"Not expected direction value: {timeInterval}"),
    };
}
