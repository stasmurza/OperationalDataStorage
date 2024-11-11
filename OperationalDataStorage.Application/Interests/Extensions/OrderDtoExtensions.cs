using OperationalDataStorage.Application.Models.Interests;
using OperationalDataStorage.Domain.Entities.Interests;

namespace OperationalDataStorage.Application.Interests.Extensions;

public static class OrderDtoExtensions
{
    public static Interest ToInterestEntity(this OrderDto dto) => new()
    {
        Symbol = dto.Symbol,
        Strategy = dto.Strategy,
        Quantity = dto.Quantity,
        AveragePrice = dto.Price,
        Direction = Enum.Parse<Domain.Entities.Direction>(dto.Direction.ToString()),
        Orders = [dto.ToOrderEntity()]
    };

    public static Order ToOrderEntity(this OrderDto dto) => new()
    {
        Symbol = dto.Symbol,
        Strategy = dto.Strategy,
        Quantity = dto.Quantity,
        Price = dto.Price,
        Direction = Enum.Parse<Domain.Entities.Direction>(dto.Direction.ToString()),
    };
}
