using MediatR;

namespace MarketDataAggregator.Models.Positions;

public class AddOwnTradesInput : IRequest
{
    public required IEnumerable<OwnTradeDto> Dtos { get; set; }
}
