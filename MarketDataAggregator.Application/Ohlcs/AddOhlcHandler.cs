using MarketDataAggregator.Application.Ohlcs.Extensions;
using MarketDataAggregator.Application.Repositories.Abstractions;
using MarketDataAggregator.Application.Models.Ohlcs;
using MediatR;
using MarketDataAggregator.Domain.Entities.Ohlcs;

namespace MarketDataAggregator.Application.Ohlcs;

public class AddOhlcHandler(IRepository<Ohlc> ohlcRepository) : IRequestHandler<AddOhlcInput, AddOhlcOutput>
{
    private readonly IRepository<Ohlc> ohlcRepository = ohlcRepository;

    public async Task<AddOhlcOutput> Handle(AddOhlcInput input, CancellationToken cancellationToken)
    {
        var interval = Enum.Parse<Domain.Entities.Ohlcs.TimeInterval>(input.Interval.ToString());
        var entity = await ohlcRepository.FirstOrDefaultAsync(i => i.StartTime == input.StartTime && i.Symbol == input.Symbol && i.Interval == interval);
        if (entity is null)
        {
            entity = Create(input);
            await ohlcRepository.CreateAsync(entity);
        }
        else
        {
            entity.Apply(input);
            await ohlcRepository.UpdateAsync(entity);
        }

        return new AddOhlcOutput { Id = entity.Id };
    }

    private static Ohlc Create(AddOhlcInput input) => new()
    {
        StartTime = input.StartTime,
        EndTime = input.EndTime,
        Symbol = input.Symbol,
        Interval = Enum.Parse<Domain.Entities.Ohlcs.TimeInterval>(input.Interval.ToString()),
        Low = input.Low,
        High = input.High,
        Open = input.Open,
        Close = input.Close,
        Volume = input.Volume,
    };
}
