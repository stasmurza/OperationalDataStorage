using MarketDataAggregator.Core.Ohlcs.Extensions;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Ohlcs;
using MarketDataAggregator.Models.Ohlcs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MarketDataAggregator.Core.Ohlcs;

public class AddOhlcHandler(ILogger<AddOhlcHandler> logger, IRepository<Ohlc> ohlcRepository) : IRequestHandler<AddOhlcInput>
{
    private readonly ILogger<AddOhlcHandler> logger = logger;
    private readonly IRepository<Ohlc> ohlcRepository = ohlcRepository;

    public async Task Handle(AddOhlcInput input, CancellationToken cancellationToken)
    {
        var interval = Enum.Parse<Entities.Ohlcs.TimeInterval>(input.Interval.ToString());
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
    }

    private async Task HandleInternalAsync(IEnumerable<OhlcDto> dtos)
    {
        var ohlcs = OhlcAggregator.Process(dtos);
        foreach (var ohlc in ohlcs)
        {
            var entity = await ohlcRepository.FirstOrDefaultAsync(i => i.StartTime == ohlc.StartTime && i.Symbol == ohlc.Symbol && i.Interval == ohlc.Interval);
            entity?.Apply(ohlc);
            if (entity is null) await ohlcRepository.CreateAsync(ohlc);
            else await ohlcRepository.UpdateAsync(ohlc);
        }
    }

    private static Ohlc Create(AddOhlcInput input) => new()
    {
        StartTime = input.StartTime,
        EndTime = input.EndTime,
        Symbol = input.Symbol,
        Interval = Enum.Parse<Entities.Ohlcs.TimeInterval>(input.Interval.ToString()),
        Low = input.Low,
        High = input.High,
        Open = input.Open,
        Close = input.Close,
        Volume = input.Volume,
    };
}
