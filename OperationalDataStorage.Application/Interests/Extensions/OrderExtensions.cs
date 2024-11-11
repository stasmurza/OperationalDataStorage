using OperationalDataStorage.Application.Models.Interests;
using OperationalDataStorage.Domain.Entities.Interests;

namespace OperationalDataStorage.Application.Interests.Extensions;

public static class OrderExtensions
{
    public static OrderDto ToOrderDto(this Order order) => new()
    {
        Id = order.Id,
        Symbol = order.Symbol,
        Strategy = order.Strategy,
        Quantity = order.Quantity,
        Price = order.Price,
        Direction = Enum.Parse<Models.Direction>(order.Direction.ToString()),
    };
}
