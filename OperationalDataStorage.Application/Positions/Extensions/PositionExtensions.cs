using OperationalDataStorage.Application.Models.Positions;
using OperationalDataStorage.Domain.Entities.Positions;

namespace OperationalDataStorage.Application.Positions.Extensions;

public static class PositionExtensions
{
    public static void Apply(this Position position, FilledOrderDto dto)
    {
        if (position.Symbol != dto.Symbol) throw new ArgumentOutOfRangeException(nameof(dto));
        if (position.Strategy != dto.Strategy) throw new ArgumentOutOfRangeException(nameof(dto));
        if (position.Orders.Any(i => i.OrderId == dto.OrderId)) return;

        var positionAmount = position.EntryPrice * position.Quantity;
        var tradeAmount = dto.AverageFillPrice * dto.FilledQuantity;
        if (position.Direction.ToString() == dto.Direction.ToString())
        {
            position.Quantity += dto.FilledQuantity;
            position.EntryPrice = (tradeAmount + positionAmount) / position.Quantity;
        }
        else
        {
            var quantity = Math.Max(dto.FilledQuantity, position.Quantity) - Math.Min(dto.FilledQuantity, position.Quantity);
            var entryPrice = quantity == 0 ? 0 : (Math.Max(positionAmount, tradeAmount) - Math.Min(positionAmount, tradeAmount)) / quantity;
            position.Quantity = quantity;
            position.EntryPrice = entryPrice;
            position.Direction = positionAmount >= tradeAmount ? position.Direction : Enum.Parse<Domain.Entities.Direction>(dto.Direction.ToString());
        }

        position.Orders.Add(new Order { OrderId = dto.OrderId });
    }

    public static PositionDto ToDto(this Position position) => new()
    {
        Id = position.Id,
        Symbol = position.Symbol,
        Strategy = position.Strategy,
        Quantity = position.Quantity,
        EntryPrice = position.EntryPrice,
        Direction = Enum.Parse<Models.Direction>(position.Direction.ToString()),
        OrderIds = [.. position.Orders.Select(i => i.OrderId)],
    };
}
