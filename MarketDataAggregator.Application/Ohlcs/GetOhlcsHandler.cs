using MarketDataAggregator.Application.Ohlcs.Extensions;
using MarketDataAggregator.Application.Repositories.Abstractions;
using MarketDataAggregator.Application.Models.Ohlcs;
using MediatR;
using MarketDataAggregator.Domain.Entities.Ohlcs;

namespace MarketDataAggregator.Application.Ohlcs;

public class GetOhlcsHandler(IRepository<Ohlc> ohlcRepository) : IRequestHandler<GetOhlcsInput, GetOhlcsOutput>
{
    private readonly IRepository<Ohlc> ohlcRepository = ohlcRepository;

    public async Task<GetOhlcsOutput> Handle(GetOhlcsInput input, CancellationToken cancellationToken)
    {
        var interval = Enum.Parse<Domain.Entities.Ohlcs.TimeInterval>(input.Granularity.ToString());
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