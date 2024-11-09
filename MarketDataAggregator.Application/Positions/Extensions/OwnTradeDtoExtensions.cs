using MarketDataAggregator.Application.Models.Positions;
using MarketDataAggregator.Domain.Entities.Positions;

namespace MarketDataAggregator.Application.Positions.Extensions;

public static class OwnTradeDtoExtensions
{
    public static Position ToPositionEntity(this OwnTradeDto dto) => new()
    {
        Symbol = dto.Symbol,
        Strategy = dto.Strategy,
        Quantity = dto.Quantity,
        EntryPrice = dto.Price,
        Direction = Enum.Parse<Domain.Entities.Direction>(dto.Direction.ToString()),
        OwnTrades = [ dto.ToOwnTradeEntity() ]
    };

    public static OwnTrade ToOwnTradeEntity(this OwnTradeDto dto) => new()
    {
        Id = dto.Id,
        DateTime = dto.DateTime,
        Symbol = dto.Symbol,
        Strategy = dto.Strategy,
        Quantity = dto.Quantity,
        Price = dto.Price,
        Direction = Enum.Parse<Domain.Entities.Direction>(dto.Direction.ToString()),
        Fee = dto.Fee,
        FeeCurrency = dto.FeeCurrency,
        OrderType = dto.OrderType,
    };
}
