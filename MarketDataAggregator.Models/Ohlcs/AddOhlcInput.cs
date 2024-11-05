using MediatR;

namespace MarketDataAggregator.Models.Ohlcs;

public struct AddOhlcInput : IRequest
{
    public required IEnumerable<OhlcDto> Dtos { get; set; }
}
