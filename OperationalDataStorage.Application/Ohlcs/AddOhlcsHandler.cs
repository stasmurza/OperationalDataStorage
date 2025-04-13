using OperationalDataStorage.Application.Ohlcs.Extensions;
using OperationalDataStorage.Application.Repositories.Abstractions;
using OperationalDataStorage.Application.Models.Ohlcs;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationalDataStorage.Domain.Entities.Ohlcs;

namespace OperationalDataStorage.Application.Ohlcs;

public class AddOhlcsHandler(ILogger<AddOhlcsHandler> logger, IRepository<Ohlc> ohlcRepository) : IRequestHandler<AddOhlcsInput>
{
    private readonly ILogger<AddOhlcsHandler> logger = logger;
    private readonly IRepository<Ohlc> ohlcRepository = ohlcRepository;
    private readonly SemaphoreSlim semaphore = new(initialCount: 1);

    public async Task Handle(AddOhlcsInput input, CancellationToken cancellationToken)
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
            var intervalStart = OhlcExtensions.GetStartDateTime(ohlc.StartTime, ohlc.Interval);
            var intervalEnd = OhlcExtensions.GetEndDateTime(ohlc.EndTime, ohlc.Interval);
            var entity = await ohlcRepository.FirstOrDefaultAsync(i => i.StartTime >= intervalStart && i.EndTime <= intervalEnd && i.Interval == ohlc.Interval && i.Symbol == ohlc.Symbol);
            entity?.Apply(ohlc);
            if (entity is null) await ohlcRepository.CreateAsync(ohlc);
            else await ohlcRepository.UpdateAsync(ohlc);
        }
    }
}
