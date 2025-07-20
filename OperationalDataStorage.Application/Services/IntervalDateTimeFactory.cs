using OperationalDataStorage.Application.Models.Ohlcs;

namespace OperationalDataStorage.Application.Services;

public static class IntervalDateTimeFactory
{
    public static DateTime GetStartDateTime(TimeInterval timeInterval, DateTime dateTime) => timeInterval switch
    {
        TimeInterval.Days1 => dateTime.Date,
        TimeInterval.Hours1 => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0),
        TimeInterval.Minutes1 => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(timeInterval), $"Not expected direction value: {timeInterval}"),
    };

    public static DateTime GetEndDateTime(TimeInterval timeInterval, DateTime dateTime) => timeInterval switch
    {
        TimeInterval.Days1 => dateTime.Date.AddDays(1).AddTicks(-1),
        TimeInterval.Hours1 => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0).AddHours(1).AddTicks(-1),
        TimeInterval.Minutes1 => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0).AddMinutes(1).AddTicks(-1),
        _ => throw new ArgumentOutOfRangeException(nameof(timeInterval), $"Not expected direction value: {timeInterval}"),
    };
}
