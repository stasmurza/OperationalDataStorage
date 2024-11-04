using EventStore.Contracts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure;

public static class EntityTypeConvertor
{
    public static EntityType GetEntityType(string routingKey) => routingKey switch
    {
        "market.data.ohlc" => EntityType.Ohlc,
        "market.order" => EntityType.MarketOrder,
        "own.trade" => EntityType.OwnTrade,
        "trading.order" => EntityType.TradingOrder,
        _ => throw new ArgumentOutOfRangeException(nameof(routingKey))
    };
}
