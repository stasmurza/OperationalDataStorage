using OperationalDataStorage.Application.Models.Ohlcs;
using OperationalDataStorage.Domain.Entities.Ohlcs;

namespace OperationalDataStorage.Application.Ohlcs.Extensions;

public static class OhlcExtensions
{
    public static void Apply(this Ohlc ohlc, OhlcDto dto)
    {
        if (ohlc.Symbol != dto.Symbol) throw new ArgumentOutOfRangeException(nameof(dto));

        if (dto.StartTime <= ohlc.StartTime)
        {
            ohlc.StartTime = dto.StartTime;
            ohlc.Open = dto.Open;
        };

        if (dto.EndTime >= ohlc.EndTime)
        {
            ohlc.EndTime = dto.EndTime;
            ohlc.Close = dto.Close;
        };

        ohlc.High = Math.Max(ohlc.High, dto.High);
        ohlc.Low = Math.Min(ohlc.Low, dto.Low);
        ohlc.Volume += dto.Volume;
    }

    public static void Apply(this Ohlc result, Ohlc ohlc)
    {
        if (result.Symbol != ohlc.Symbol) throw new ArgumentOutOfRangeException(nameof(ohlc));
        if (ohlc.EndTime <= result.EndTime) return;

        if (result.StartTime <= ohlc.StartTime)
        {
            result.StartTime = ohlc.StartTime;
            result.Open = ohlc.Open;
        };

        if (result.EndTime >= ohlc.EndTime)
        {
            result.EndTime = ohlc.EndTime;
            result.Close = ohlc.Close;
        };

        result.High = Math.Max(ohlc.High, ohlc.High);
        result.Low = Math.Min(ohlc.Low, ohlc.Low);
        result.Volume += ohlc.Volume;
    }

    public static void Apply(this Ohlc result, AddOhlcInput input)
    {
        if (result.Symbol != input.Symbol) throw new ArgumentOutOfRangeException(nameof(input));
        if (input.EndTime <= result.EndTime) return;

        if (result.StartTime <= input.StartTime)
        {
            result.StartTime = input.StartTime;
            result.Open = input.Open;
        };

        if (result.EndTime >= input.EndTime)
        {
            result.EndTime = input.EndTime;
            result.Close = input.Close;
        };

        result.High = Math.Max(input.High, input.High);
        result.Low = Math.Min(input.Low, input.Low);
        result.Volume += input.Volume;
    }

    public static OhlcDto ToDto(this Ohlc ohlc) => new()
    {
        Id = ohlc.Id,
        StartTime = ohlc.StartTime,
        EndTime = ohlc.EndTime,
        Symbol = ohlc.Symbol,
        Interval = Enum.Parse<Models.Ohlcs.TimeInterval>(ohlc.Interval.ToString()),
        Low = ohlc.Low,
        High = ohlc.High,
        Open = ohlc.Open,
        Close = ohlc.Close,
        Volume = ohlc.Volume,
    };

    public static string GetKey(this Ohlc ohlc)
    {
        return ohlc.Interval.ToString() + GetStartDateTime(ohlc.StartTime, ohlc.Interval).ToString() + ohlc.Symbol;
    }

    public static string GetKey(Domain.Entities.Ohlcs.TimeInterval timeInterval, DateTime intervalStart, string symbol)
    {
        return timeInterval.ToString() + intervalStart.ToString() + symbol;
    }

    public static DateTime GetStartDateTime(DateTime startTime, Domain.Entities.Ohlcs.TimeInterval timeInterval) => timeInterval switch
    {
        Domain.Entities.Ohlcs.TimeInterval.Days1 => startTime.Date,
        Domain.Entities.Ohlcs.TimeInterval.Hours1 => new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0),
        Domain.Entities.Ohlcs.TimeInterval.Minutes1 => new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(timeInterval), $"Not expected direction value: {timeInterval}"),
    };

    public static DateTime GetEndDateTime(DateTime startTime, Domain.Entities.Ohlcs.TimeInterval timeInterval) => timeInterval switch
    {
        Domain.Entities.Ohlcs.TimeInterval.Days1 => startTime.Date.AddDays(1).AddTicks(-1),
        Domain.Entities.Ohlcs.TimeInterval.Hours1 => new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0).AddHours(1).AddTicks(-1),
        Domain.Entities.Ohlcs.TimeInterval.Minutes1 => new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0).AddMinutes(1).AddTicks(-1),
        _ => throw new ArgumentOutOfRangeException(nameof(timeInterval), $"Not expected direction value: {timeInterval}"),
    };
}
