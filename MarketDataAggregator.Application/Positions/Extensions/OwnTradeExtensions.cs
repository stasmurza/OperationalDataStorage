using MarketDataAggregator.Application.Models.Positions;
using MarketDataAggregator.Domain.Entities.Positions;

namespace MarketDataAggregator.Application.Positions.Extensions;

public static class OwnTradeExtensions
{
    public static OwnTradeDto ToOwnTradeDto(this OwnTrade ownTrade) => new()
    {
        Id = ownTrade.Id,
        DateTime = ownTrade.DateTime,
        Symbol = ownTrade.Symbol,
        Strategy = ownTrade.Strategy,
        Quantity = ownTrade.Quantity,
        Price = ownTrade.Price,
        Direction = Enum.Parse<Models.Direction>(ownTrade.Direction.ToString()),
        Fee = ownTrade.Fee,
        FeeCurrency = ownTrade.FeeCurrency,
        OrderType = ownTrade.OrderType,
    };
}
