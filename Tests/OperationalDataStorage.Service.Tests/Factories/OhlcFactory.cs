using OperationalDataStorage.Contracts.Ohlcs;

namespace OperationalDataStorage.Service.Tests.Factories;

/// <summary>
/// Ohlc factory.
/// </summary>
public static class OhlcFactory
{
    /// <summary>
    /// Creates Ohlc.
    /// </summary>
    public static Ohlc CreateOhlc(string symbol, DateTime startInterval, DateTime endInterval, TimeInterval granularity, int minPrice, int maxPrice, decimal volume)
    {
        var random = new Random();
        var price1 = random.Next(minPrice, maxPrice);
        var price2 = random.Next(minPrice, maxPrice);
        var low = Math.Min(price1, price2);
        var high = Math.Max(price1, price2);
        return new Ohlc()
        {
            StartTime = startInterval,
            EndTime = endInterval,
            Symbol = symbol,
            Interval = TimeInterval.Days1,
            Low = low,
            High = high,
            Open = random.Next(low, high),
            Close = random.Next(low, high),
            Volume = volume,
        };
    }

    /// <summary>
    /// Creates Ohlcs.
    /// </summary>
    public static IEnumerable<Ohlc> CreateOhlcs(string symbol, DateTime start, DateTime end, TimeInterval granularity, int minPrice, int maxPrice)
    {
        var random = new Random();
        var date = start;
        while (date <= end)
        {
            var price1 = random.Next(minPrice, maxPrice);
            var price2 = random.Next(minPrice, maxPrice);
            var low = Math.Min(price1, price2);
            var high = Math.Max(price1, price2);
            yield return new Ohlc()
            {
                StartTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0),
                EndTime = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59),
                Symbol = symbol,
                Interval = TimeInterval.Days1,
                Low = low,
                High = high,
                Open = random.Next(low, high),
                Close = random.Next(low, high),
                Volume = random.Next(1000, 10000),
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
