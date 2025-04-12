using OperationalDataStorage.Application.Models.Positions;
using OperationalDataStorage.Domain.Entities.Positions;

namespace OperationalDataStorage.Application.Positions.Extensions;

public static class FilledOrderDtoExtensions
{
    public static Position ToPositionEntity(this FilledOrderDto dto) => new()
    {
        Symbol = dto.Symbol,
        Strategy = dto.Strategy,
        Quantity = dto.FilledQuantity,
        EntryPrice = dto.AverageFillPrice,
        Direction = Enum.Parse<Domain.Entities.Direction>(dto.Direction.ToString()),
        Orders = [ dto.ToOwnTradeEntity() ]
    };

    public static OwnTrade ToOwnTradeEntity(this FilledOrderDto dto) => new()
    {
        Id = dto.OrderId,
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
