using OperationalDataStorage.Application.Models.Positions;
using OperationalDataStorage.Domain.Entities.Positions;

namespace OperationalDataStorage.Application.Positions.Extensions;

public static class FilledOrderExtensions
{
    public static OrderDto ToOrderDto(this Order order) => new()
    {
        OrderId = order.OrderId
    };
}
