using MarketDataAggregator.Models.Ohlcs;
using MediatR;

namespace MarketDataAggregator.Models.Interests;

public class GetInterestsInput : IRequest<GetInterestsOutput>
{
    public required string Strategy { get; set; }
}
