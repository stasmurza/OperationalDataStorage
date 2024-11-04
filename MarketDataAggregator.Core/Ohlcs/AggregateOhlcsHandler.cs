using MarketDataAggregator.Core.Ohlcs.Aggregates;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Models.Entities.Events;
using MarketDataAggregator.Models.Ohlcs.Aggregates;
using MediatR;
using System.Text.Json;

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
