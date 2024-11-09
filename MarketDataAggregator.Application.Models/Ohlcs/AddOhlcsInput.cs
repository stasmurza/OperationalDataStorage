using MediatR;

namespace MarketDataAggregator.Application.Models.Ohlcs;

public struct AddOhlcsInput : IRequest
{
    public required IEnumerable<OhlcDto> Dtos { get; set; }
}
