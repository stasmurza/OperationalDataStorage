using MediatR;

namespace MarketDataAggregator.Application.Models.Positions;

public class AddOwnTradesInput : IRequest
{
    public required IEnumerable<OwnTradeDto> Dtos { get; set; }
}
