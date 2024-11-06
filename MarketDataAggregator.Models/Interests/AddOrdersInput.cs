using MediatR;

namespace MarketDataAggregator.Models.Interests;

public class AddOrdersInput : IRequest
{
    public required IEnumerable<OrderDto> Dtos { get; set; }
}
