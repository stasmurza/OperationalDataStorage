using MarketDataAggregator.Application.Models.Ohlcs;
using MediatR;

namespace MarketDataAggregator.Application.Models.Interests;

public class GetInterestsInput : IRequest<GetInterestsOutput>
{
    public required string Strategy { get; set; }
}
