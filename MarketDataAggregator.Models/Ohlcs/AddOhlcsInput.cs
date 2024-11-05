using MediatR;

namespace MarketDataAggregator.Models.Ohlcs;

public struct AddOhlcsInput : IRequest
{
    public required IEnumerable<OhlcDto> Dtos { get; set; }
}
