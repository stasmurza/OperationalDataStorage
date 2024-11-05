using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Ohlcs;
using MarketDataAggregator.Entities.Positions;
using MarketDataAggregator.Models.Ohlcs;
using MarketDataAggregator.Models.OwnTrades;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MarketDataAggregator.Core.Positions;

public class AddOwnTradesHandler
{
    private readonly ILogger<AddOwnTradesHandler> logger;
    private readonly IContext context;
    private readonly IRepository<Position> positionRepository;
    private readonly SemaphoreSlim semaphore = new(initialCount: 1, maxCount: 1);

    public AddOwnTradesHandler(
        ILogger<AddOwnTradesHandler> logger,
        IContext context,
        IRepository<Position> positionRepository)
    {
        this.logger = logger;
        this.context = context;
        this.positionRepository = positionRepository;
    }

    public async Task Handle(AddOwnTradesInput input, CancellationToken cancellationToken)
    {
        var entity = await positionRepository.FirstOrDefaultAsync(i => i.Symbol == input.StartTime && i.Symbol == input.Symbol && i.Interval == interval);
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

    public async Task AddOwnTrade(OwnTradeDto dto, CancellationToken cancellationToken)
    {
        var entity = await positionRepository.FirstOrDefaultAsync(i => i.Symbol == dto.Symbol && i.Strategy == dto.Strategy);
        if (entity is null)
        {
            entity = Create(dto);
            await positionRepository.CreateAsync(entity);
        }
        else
        {
            entity.Apply(dto);
            await ohlcRepository.UpdateAsync(entity);
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

    private static Position Create(OwnTradeDto dto) => new()
    {
        Symbol = dto.Symbol,
        Strategy = dto.Strategy,
        Quantity = dto.Quantity,
        EntryPrice = dto.Price,
        Direction = Enum.Parse<Entities.Direction>(dto.Direction.ToString()),
    };
}
