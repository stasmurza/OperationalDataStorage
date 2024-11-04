using EventStore.Contracts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure;

public static class EventTypeConvertor
{
    public static EventType GetEventType(string routingKey) => routingKey switch
    {
        "market.data.ohlc" => EventType.OhlcReceived,
        "market.order" => EventType.MarketOrderReceived,
        "own.trade" => EventType.OwnTradeReceived,
        "trading.order" => EventType.TradingOrder,
        _ => throw new ArgumentOutOfRangeException(nameof(routingKey))
    };
}
