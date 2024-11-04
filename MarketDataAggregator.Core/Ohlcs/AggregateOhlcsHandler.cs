using MarketDataAggregator.Models.Ohlcs.Aggregates;
using MediatR;

namespace MarketDataAggregator.Core.Ohlcs;

public class AggregateOhlcsHandler(OhlcAggregateBuilder ohlcAggregateBuilder) :
    IRequestHandler<AggregateOhlcsInput>
{
    private readonly OhlcAggregateBuilder ohlcAggregateBuilder = ohlcAggregateBuilder;

    public async Task Handle(AggregateOhlcsInput input, CancellationToken cancellationToken)
    {
        await ohlcAggregateBuilder.ProcessAsync();
    }
}
