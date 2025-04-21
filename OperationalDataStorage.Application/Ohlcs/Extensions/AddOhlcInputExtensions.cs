using OperationalDataStorage.Application.Models.Ohlcs;

namespace OperationalDataStorage.Application.Ohlcs.Extensions;

public static class AddOhlcInputExtensions
{
    public static DateTime GetStartDateTime(this AddOhlcInput input) => input.Interval switch
    {
        TimeInterval.Days1 => input.StartTime.Date,
        TimeInterval.Hours1 => new DateTime(input.StartTime.Year, input.StartTime.Month, input.StartTime.Day, input.StartTime.Hour, 0, 0),
        TimeInterval.Minutes1 => new DateTime(input.StartTime.Year, input.StartTime.Month, input.StartTime.Day, input.StartTime.Hour, 0, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(input.Interval), $"Not expected direction value: {input.Interval}"),
    };

    public static DateTime GetEndDateTime(this AddOhlcInput input) => input.Interval switch
    {
        TimeInterval.Days1 => input.StartTime.Date.AddDays(1).AddTicks(-1),
        TimeInterval.Hours1 => new DateTime(input.StartTime.Year, input.StartTime.Month, input.StartTime.Day, input.StartTime.Hour, 0, 0).AddHours(1).AddTicks(-1),
        TimeInterval.Minutes1 => new DateTime(input.StartTime.Year, input.StartTime.Month, input.StartTime.Day, input.StartTime.Hour, 0, 0).AddMinutes(1).AddTicks(-1),
        _ => throw new ArgumentOutOfRangeException(nameof(input.Interval), $"Not expected direction value: {input.Interval}"),
    };
}
