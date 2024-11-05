using MarketDataAggregator.Models.Ohlcs;
using MediatR;

namespace MarketDataAggregator.Models.Interests;

public class GetInterestInput : IRequest<GetInterestOutput>
{
    public required string Strategy { get; set; }
}
