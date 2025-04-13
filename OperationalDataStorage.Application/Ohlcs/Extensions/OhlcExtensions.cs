using OperationalDataStorage.Application.Models.Ohlcs;
using OperationalDataStorage.Domain.Entities.Ohlcs;

namespace OperationalDataStorage.Application.Ohlcs.Extensions;

public static class OhlcExtensions
{
    public static void Apply(this Ohlc ohlc, OhlcDto dto)
    {
        if (ohlc.Symbol != dto.Symbol) throw new ArgumentOutOfRangeException(nameof(dto));
        if (ohlc.Volume >= dto.Volume) return;

        ohlc.StartTime = dto.StartTime;
        ohlc.EndTime = dto.EndTime;
        ohlc.Open = dto.Open;
        ohlc.Close = dto.Close;
        ohlc.High = dto.High;
        ohlc.Low = dto.Low;
        ohlc.Volume = dto.Volume;
    }

    public static void Apply(this Ohlc result, Ohlc ohlc)
    {
        if (result.Symbol != ohlc.Symbol) throw new ArgumentOutOfRangeException(nameof(ohlc));
        if (result.Volume >= ohlc.Volume) return;

        result.StartTime = ohlc.StartTime;
        result.EndTime = ohlc.EndTime;
        result.Open = ohlc.Open;
        result.Close = ohlc.Close;
        result.High = ohlc.High;
        result.Low = ohlc.Low;
        result.Volume = ohlc.Volume;
    }

    public static void Apply(this Ohlc result, AddOhlcInput input)
    {
        if (result.Symbol != input.Symbol) throw new ArgumentOutOfRangeException(nameof(input));
        if (result.Volume >= input.Volume) return;

        result.StartTime = input.StartTime;
        result.EndTime = input.EndTime;
        result.Open = input.Open;
        result.Close = input.Close;
        result.High = input.High;
        result.Low = input.Low;
        result.Volume = input.Volume;
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

    public static DateTime GetStartDateTime(this Ohlc ohlc) => ohlc.Interval switch
    {
        Domain.Entities.Ohlcs.TimeInterval.Days1 => ohlc.StartTime.Date,
        Domain.Entities.Ohlcs.TimeInterval.Hours1 => new DateTime(ohlc.StartTime.Year, ohlc.StartTime.Month, ohlc.StartTime.Day, ohlc.StartTime.Hour, 0, 0),
        Domain.Entities.Ohlcs.TimeInterval.Minutes1 => new DateTime(ohlc.StartTime.Year, ohlc.StartTime.Month, ohlc.StartTime.Day, ohlc.StartTime.Hour, 0, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(ohlc.Interval), $"Not expected direction value: {ohlc.Interval}"),
    };

    public static DateTime GetEndDateTime(this Ohlc ohlc) => ohlc.Interval switch
    {
        Domain.Entities.Ohlcs.TimeInterval.Days1 => ohlc.StartTime.Date.AddDays(1).AddTicks(-1),
        Domain.Entities.Ohlcs.TimeInterval.Hours1 => new DateTime(ohlc.StartTime.Year, ohlc.StartTime.Month, ohlc.StartTime.Day, ohlc.StartTime.Hour, 0, 0).AddHours(1).AddTicks(-1),
        Domain.Entities.Ohlcs.TimeInterval.Minutes1 => new DateTime(ohlc.StartTime.Year, ohlc.StartTime.Month, ohlc.StartTime.Day, ohlc.StartTime.Hour, 0, 0).AddMinutes(1).AddTicks(-1),
        _ => throw new ArgumentOutOfRangeException(nameof(ohlc.Interval), $"Not expected direction value: {ohlc.Interval}"),
    };
}
