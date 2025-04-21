using OperationalDataStorage.Application.Models.Ohlcs;

namespace OperationalDataStorage.Application.Ohlcs.Extensions;

public static class OhlcDtoExtensions
{
    public static DateTime GetStartDateTime(this OhlcDto dto) => dto.Interval switch
    {
        TimeInterval.Days1 => dto.StartTime.Date,
        TimeInterval.Hours1 => new DateTime(dto.StartTime.Year, dto.StartTime.Month, dto.StartTime.Day, dto.StartTime.Hour, 0, 0),
        TimeInterval.Minutes1 => new DateTime(dto.StartTime.Year, dto.StartTime.Month, dto.StartTime.Day, dto.StartTime.Hour, 0, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(dto.Interval), $"Not expected direction value: {dto.Interval}"),
    };

    public static DateTime GetEndDateTime(this OhlcDto dto) => dto.Interval switch
    {
        TimeInterval.Days1 => dto.StartTime.Date.AddDays(1).AddTicks(-1),
        TimeInterval.Hours1 => new DateTime(dto.StartTime.Year, dto.StartTime.Month, dto.StartTime.Day, dto.StartTime.Hour, 0, 0).AddHours(1).AddTicks(-1),
        TimeInterval.Minutes1 => new DateTime(dto.StartTime.Year, dto.StartTime.Month, dto.StartTime.Day, dto.StartTime.Hour, 0, 0).AddMinutes(1).AddTicks(-1),
        _ => throw new ArgumentOutOfRangeException(nameof(dto.Interval), $"Not expected direction value: {dto.Interval}"),
    };
}
