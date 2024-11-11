namespace OperationalDataStorage.Contracts.Ohlcs;

/// <summary>
/// Get Ohlcs response.
/// </summary>
public struct GetOhlcsResponse
{
    /// <summary>
    /// Ohlcs.
    /// </summary>
    public IEnumerable<OhlcDto> Ohlcs { get; set; }
}
