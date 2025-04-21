namespace OperationalDataStorage.Contracts.Events;

/// <summary>
/// Type of event.
/// </summary>
public enum EventType
{
    /// <summary>
    /// Ohlc received.
    /// </summary>
    OhlcReceived,

    /// <summary>
    /// Own trade received.
    /// </summary>
    FilledOrderReceived,
}
