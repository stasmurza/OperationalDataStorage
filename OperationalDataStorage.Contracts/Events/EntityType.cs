namespace OperationalDataStorage.Contracts.Events;

/// <summary>
/// Entity type.
/// </summary>
public enum EntityType
{
    /// <summary>
    /// Ohlc.
    /// </summary>
    Ohlc,

    /// <summary>
    /// Own trade.
    /// </summary>
    OwnTrade,

    /// <summary>
    /// Trading order.
    /// </summary>
    TradingOrder,

    /// <summary>
    /// Market order.
    /// </summary>
    MarketOrder,
}
