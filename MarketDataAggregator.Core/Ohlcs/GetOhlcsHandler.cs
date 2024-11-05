using MarketDataAggregator.Core.Ohlcs.Extensions;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Ohlcs;
using MarketDataAggregator.Models.Ohlcs;
using MediatR;

namespace MarketDataAggregator.Core.Ohlcs;

public class GetOhlcsHandler(IRepository<Ohlc> ohlcRepository) : IRequestHandler<GetOhlcsInput, GetOhlcsOutput>
{
    private readonly IRepository<Ohlc> ohlcRepository = ohlcRepository;

    public async Task<GetOhlcsOutput> Handle(GetOhlcsInput input, CancellationToken cancellationToken)
    {
        var interval = Enum.Parse<Entities.Ohlcs.TimeInterval>(input.Granularity.ToString());
        var ohlcs = await ohlcRepository.GetAsync(i =>
            i.Symbol == input.Symbol &&
            i.Interval == interval &&
            i.StartTime >= input.Start &&
            i.StartTime <= input.End);

        return new GetOhlcsOutput()
        {
            Ohlcs = ohlcs.Select(i => i.ToDto()),
        };
    }
}