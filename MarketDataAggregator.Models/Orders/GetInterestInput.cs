using MarketDataAggregator.Models.Ohlcs;
using MediatR;

namespace MarketDataAggregator.Models.Orders;

public class GetInterestInput : IRequest<GetOhlcsOutput>
{
    public required string Strategy { get; set; }
}
