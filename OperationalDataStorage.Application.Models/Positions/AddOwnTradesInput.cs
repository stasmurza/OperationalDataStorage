using MediatR;

namespace OperationalDataStorage.Application.Models.Positions;

public class AddOwnTradesInput : IRequest
{
    public required IEnumerable<OwnTradeDto> Dtos { get; set; }
}
