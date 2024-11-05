using MarketDataAggregator.Models.Aggregates;
using MediatR;

namespace MarketDataAggregator.Core.Ohlcs;

public class AggregateOhlcsHandler(OhlcAggregator ohlcAggregateBuilder) :
    IRequestHandler<AggregateOhlcsInput>
{
    private readonly OhlcAggregator ohlcAggregateBuilder = ohlcAggregateBuilder;

    public async Task Handle(AggregateOhlcsInput input, CancellationToken cancellationToken)
    {
        await ohlcAggregateBuilder.ProcessAsync();
    }
}
