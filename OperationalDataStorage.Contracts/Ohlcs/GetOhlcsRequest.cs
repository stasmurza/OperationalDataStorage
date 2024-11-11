namespace OperationalDataStorage.Contracts.Ohlcs;

/// <summary>
/// Get Ohlcs request.
/// </summary>
public class GetOhlcsRequest
{
    /// <summary>
    /// Symbol.
    /// </summary>
    /// <example>BTC/USDT</example>
    public required string Symbol { get; set; }

    /// <summary>
    /// Start.
    /// </summary>
    /// <example>2009-06-15T13:45:00</example>
    public required DateTime Start { get; set; }

    /// <summary>
    /// End.
    /// </summary>
    /// <example>2009-06-16T13:45:00</example>
    public required DateTime End { get; set; }

    /// <summary>
    /// Granularity.
    /// </summary>
    /// <example>Minutes1</example>
    public required TimeInterval Granularity { get; set; }
}
