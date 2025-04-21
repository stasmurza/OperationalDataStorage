using MediatR;

namespace OperationalDataStorage.Application.Models.Ohlcs;

public struct AddOhlcsInput : IRequest
{
    public required IEnumerable<OhlcInputDto> Dtos { get; set; }
}
