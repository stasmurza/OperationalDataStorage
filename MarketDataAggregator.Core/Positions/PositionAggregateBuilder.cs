using MarketDataAggregator.Core.Ohlcs.Aggregates;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Models.Entities.Events;
using MarketDataAggregator.Models.Entities.Ohlcs;
using MarketDataAggregator.Models.Ohlcs.Aggregates;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MarketDataAggregator.Core.Positions;

public class PositionAggregateBuilder
{
    private readonly ILogger<PositionAggregateBuilder> logger;
    private readonly IContext context;
    private readonly IRepository<Ohlc> ohlcRepository;
    private readonly IRepository<Event> eventRepository;
    private readonly SemaphoreSlim semaphore = new(initialCount: 1, maxCount: 1);

    public PositionAggregateBuilder(
        ILogger<PositionAggregateBuilder> logger,
        IContext context,
        IRepository<Ohlc> ohlcRepository,
        IRepository<Event> eventRepository)
    {
        this.logger = logger;
        this.context = context;
        this.ohlcRepository = ohlcRepository;
        this.eventRepository = eventRepository;
    }

    public async Task ProcessAsync()
    {
        try
        {
            await semaphore.WaitAsync();
            await ProcessInternalAsync();
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

    private async Task ProcessInternalAsync()
    {
        var events = await eventRepository.GetAsync(i => !i.Processed && i.EventType == EventType.OhlcReceived.ToString());
        var eventList = events.ToList();

        List<OhlcDto> historicalExchangeRates = eventList
            .Select(i => JsonSerializer.Deserialize<OhlcDto>(i.EventData) ?? throw new NullReferenceException(nameof(OhlcDto)))
            .ToList();

        if (historicalExchangeRates.Count == 0) return;

        var dictionary = historicalExchangeRates
            .GroupBy(i => new
            {
                i.StartTime,
                i.Symbol
            })
            .ToDictionary(i => i.Key, i => i.ToList());

        var intervals = new List<TimeInterval>()
        {
            TimeInterval.Days1,
            TimeInterval.Hours1,
            TimeInterval.Minutes1
        };

        var ohlcs = new List<Ohlc>();
        foreach (var keyValuePair in dictionary)
        {
            var exchangeRates = keyValuePair.Value;
            if (exchangeRates.Count == 0) continue;

            var result = UpdateAggregatesAsync(keyValuePair.Key.StartTime, keyValuePair.Key.Symbol, intervals, exchangeRates);
            await foreach (var item in result) ohlcs.Add(item);
        }

        await SaveChanges(ohlcs, eventList);

        foreach (var ohlc in ohlcs)
        {
            var eventArgs = new AggregateUpdatedEventArgs()
            {
                Ohlc = ohlc
            };
            AggregateUpdated?.Invoke(this, eventArgs);
        }
    }

    private async Task SaveChanges(List<Ohlc> ohlcs, List<Event> events)
    {
        foreach (var ohlc in ohlcs)
        {
            if (string.IsNullOrEmpty(ohlc.Id)) await ohlcRepository.CreateAsync(ohlc);
            else await ohlcRepository.UpdateAsync(ohlc);
        }

        foreach (var @event in events)
        {
            @event.Processed = true;
            await eventRepository.UpdateAsync(@event);
        }
    }

    private async IAsyncEnumerable<Ohlc> UpdateAggregatesAsync(DateTime startTime, string symbol, IEnumerable<TimeInterval> timeIntervals, IEnumerable<OhlcDto> historicalExchangeRates)
    {
        foreach (var timeInterval in timeIntervals)
        {
            var ohlc = await ohlcRepository.FirstOrDefaultAsync(i =>
            i.StartTime == startTime &&
            i.Symbol == symbol &&
            i.Interval == timeInterval);

            var applicableRates = historicalExchangeRates.Where(i => (int)i.Interval <= (int)timeInterval);
            if (!applicableRates.Any()) continue;

            foreach (var historicalExchangeRate in applicableRates)
            {
                if (ohlc == null) ohlc = Create(historicalExchangeRate, startTime, timeInterval);
                else ohlc.Apply(historicalExchangeRate);
            }

            yield return ohlc!;
        }
    }

    private static Ohlc Create(OhlcDto historicalExchangeRate, DateTime startTime, TimeInterval interval) => new()
    {
        Symbol = historicalExchangeRate.Symbol,
        Interval = interval,
        MinTime = historicalExchangeRate.StartTime,
        MaxTime = historicalExchangeRate.StartTime,
        StartTime = startTime,
        Low = historicalExchangeRate.Low,
        High = historicalExchangeRate.High,
        Open = historicalExchangeRate.Open,
        Close = historicalExchangeRate.Close,
        Volume = historicalExchangeRate.Volume,
    };
}
