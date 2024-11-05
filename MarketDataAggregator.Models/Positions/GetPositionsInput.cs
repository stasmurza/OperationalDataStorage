using MediatR;

namespace MarketDataAggregator.Models.Positions;

public class GetPositionsInput : IRequest<GetPositionsOutput>
{
    public required string Strategy { get; set; }
}
