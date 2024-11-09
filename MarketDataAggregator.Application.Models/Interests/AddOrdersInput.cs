using MediatR;

namespace MarketDataAggregator.Application.Models.Interests;

public class AddOrdersInput : IRequest
{
    public required IEnumerable<OrderDto> Dtos { get; set; }
}
