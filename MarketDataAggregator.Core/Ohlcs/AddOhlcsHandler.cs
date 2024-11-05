using MarketDataAggregator.Core.Ohlcs.Extensions;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Ohlcs;
using MarketDataAggregator.Models.Ohlcs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MarketDataAggregator.Core.Ohlcs;

public class AddOhlcsHandler(ILogger<AddOhlcsHandler> logger, IRepository<Ohlc> ohlcRepository) : IRequestHandler<AddOhlcInput>
{
    private readonly ILogger<AddOhlcsHandler> logger = logger;
    private readonly IRepository<Ohlc> ohlcRepository = ohlcRepository;
    private readonly SemaphoreSlim semaphore = new(initialCount: 1);

    public async Task Handle(AddOhlcInput input, CancellationToken cancellationToken)
    {
        try
        {
            await semaphore.WaitAsync(cancellationToken);
            await HandleInternalAsync(input.Dtos);
        }
        catch (Exception exception)
        {
            logger.LogError("{errorMessage}", exception.Message);
            throw;
        }
        finally
        {
            semaphore.Release();
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
}
