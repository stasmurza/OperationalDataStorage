using MediatR;

namespace OperationalDataStorage.Application.Models.Interests;

public class AddOrdersInput : IRequest
{
    public required IEnumerable<OrderDto> Dtos { get; set; }
}
