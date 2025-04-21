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
        OrderIds = [ dto.OrderId ]
    };
}
