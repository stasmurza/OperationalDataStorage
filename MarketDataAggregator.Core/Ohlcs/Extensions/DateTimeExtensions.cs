using MarketDataAggregator.Models;
using Microsoft.VisualBasic;

namespace MarketDataAggregator.Core.Ohlcs.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToStartTime(this DateTime dateTime, TimeInterval timeInterval)
    {
        return timeInterval switch
        {
            TimeInterval.Days1 => dateTime.Date,
            TimeInterval.Hours1 => new DateTime(DateOnly.FromDateTime(dateTime), new TimeOnly(dateTime.Hour, 0)),
            TimeInterval.Minutes1 => new DateTime(DateOnly.FromDateTime(dateTime), new TimeOnly(dateTime.Hour, dateTime.Minute)),
            _ => throw new ArgumentOutOfRangeException(nameof(dateTime)),
        };
    }
}
