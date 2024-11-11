using MediatR;

namespace OperationalDataStorage.Application.Models.Ohlcs;

public struct AddOhlcsInput : IRequest
{
    public required IEnumerable<OhlcDto> Dtos { get; set; }
}
