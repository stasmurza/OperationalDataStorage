namespace MarketDataAggregator.Contracts.Ohlcs;

/// <summary>
/// Ohlc DTO.
/// </summary>
public class OhlcDto
{
    /// <summary>
    /// Start of the interval time.
    /// </summary>
    public required DateTime StartTime { get; set; }

    /// <summary>
    /// Time of the last trade.
    /// </summary>
    public required DateTime EndTime { get; set; }

    /// <summary>
    /// Symbol.
    /// </summary>
    public required string Symbol { get; set; }

    /// <summary>
    /// Interval.
    /// </summary>
    public required TimeInterval Interval { get; set; }

    /// <summary>
    /// Low.
    /// </summary>
    public decimal Low { get; set; }

    /// <summary>
    /// High.
    /// </summary>
    public decimal High { get; set; }

    /// <summary>
    /// Open.
    /// </summary>
    public decimal Open { get; set; }

    /// <summary>
    /// Close.
    /// </summary>
    public decimal Close { get; set; }

    /// <summary>
    /// Volume.
    /// </summary>
    public decimal Volume { get; set; }
}
