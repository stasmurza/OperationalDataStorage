using MarketDataAggregator.Core.Ohlcs.Converters;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Models.Entities.Events;
using MarketDataAggregator.Models.Ohlcs;
using MediatR;
using System.Text.Json;

namespace MarketDataAggregator.Core.Ohlcs;

public class AddOhlcHandler(IRepository<Event> eventRepository) : IRequestHandler<AddOhlcInput>
{
    private readonly IRepository<Event> eventRepository = eventRepository;

    public async Task Handle(AddOhlcInput input, CancellationToken cancellationToken)
    {
        var historicalExchangeRate = ModelConverter.Convert(input);
        var eventObject = new Event
        {
            DateTime = DateTime.UtcNow,
            EventType = EventType.OhlcReceived.ToString(),
            EntityType = EntityType.Ohlc1Minute.ToString(),
            EntityId = input.Symbol,
            EventData = JsonSerializer.Serialize(historicalExchangeRate),
        };
        await eventRepository.CreateAsync(eventObject);
    }
}
