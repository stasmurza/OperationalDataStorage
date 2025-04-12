using OperationalDataStorage.Contracts.Ohlcs;

namespace OperationalDataStorage.Service.Tests.Factories;

/// <summary>
/// Ohlc factory.
/// </summary>
public static class OhlcFactory
{
    /// <summary>
    /// Creates Ohlcs.
    /// </summary>
    public static IEnumerable<Ohlc> CreateOhlcs(string symbol, DateTime start, DateTime end, TimeInterval granularity)
    {
        var price = 60000;
        var date = start;
        while (date <= end)
        {
            yield return new Ohlc()
            {
                StartTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0),
                EndTime = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59),
                Symbol = symbol,
                Interval = TimeInterval.Days1,
                Low = price - 1,
                High = price + 1,
                Open = price,
                Close = price,
                Volume = 10000,
            };

            date = GetNextDateTime(date, granularity);
        }
    }

    private static DateTime GetNextDateTime(DateTime dateTime, TimeInterval granularity) => granularity switch
    {
        TimeInterval.Days1 => dateTime.AddDays(1),
        TimeInterval.Minutes1 => dateTime.AddMinutes(1),
        _ => throw new ArgumentOutOfRangeException(nameof(granularity))
    };
}
