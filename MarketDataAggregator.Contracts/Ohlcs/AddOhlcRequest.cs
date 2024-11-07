namespace MarketDataAggregator.Contracts.Ohlcs;

/// <summary>
/// Add Ohlc request.
/// </summary>
public class AddOhlcRequest
{
    /// <summary>
    /// Start of the interval time.
    /// </summary>
    /// <example>2009-06-15T13:45:00</example>
    public required DateTime StartTime { get; set; }

    /// <summary>
    /// Time of the last trade.
    /// </summary>
    /// <example>2009-06-15T13:45:59</example>
    public required DateTime EndTime { get; set; }

    /// <summary>
    /// Symbol.
    /// </summary>
    /// <example>BTC/USDT</example>
    public required string Symbol { get; set; }

    /// <summary>
    /// Interval.
    /// </summary>
    /// <example>Minutes1</example>
    public required TimeInterval Interval { get; set; }

    /// <summary>
    /// Low.
    /// </summary>
    /// <example>65000</example>
    public required decimal Low { get; set; }

    /// <summary>
    /// High.
    /// </summary>
    /// <example>65100</example>
    public required decimal High { get; set; }

    /// <summary>
    /// Open.
    /// </summary>
    /// <example>65010</example>
    public required decimal Open { get; set; }

    /// <summary>
    /// Close.
    /// </summary>
    /// <example>65080</example>
    public required decimal Close { get; set; }

    /// <summary>
    /// Volume.
    /// </summary>
    /// <example>30</example>
    public required decimal Volume { get; set; }
}
