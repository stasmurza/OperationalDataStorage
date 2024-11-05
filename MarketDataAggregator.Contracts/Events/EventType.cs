namespace MarketDataAggregator.Contracts.Events;

public enum EventType
{
    /// <summary>
    /// Market order received.
    /// </summary>
    MarketOrderReceived,

    /// <summary>
    /// Ohlc received.
    /// </summary>
    OhlcReceived,

    /// <summary>
    /// Own trade received.
    /// </summary>
    OwnTradeReceived,

    /// <summary>
    /// Trading order received.
    /// </summary>
    TradingOrderReceived,
}
