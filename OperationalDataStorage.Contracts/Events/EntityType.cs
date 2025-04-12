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
    FilledOrder,

    /// <summary>
    /// Trade request.
    /// </summary>
    TradeRequest,

    /// <summary>
    /// Market order.
    /// </summary>
    MarketOrder,
}
