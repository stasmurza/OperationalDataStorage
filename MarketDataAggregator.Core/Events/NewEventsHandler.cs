using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Models.Entities.Events;
using MarketDataAggregator.Models.Events;
using MediatR;

namespace MarketDataAggregator.Core.Events;

public class NewEventsHandler(IRepository<Event> eventRepository) : IRequestHandler<NewEventsInput>
{
    private readonly IRepository<Event> eventRepository = eventRepository;

    public async Task Handle(NewEventsInput input, CancellationToken cancellationToken)
    {
        var eventObject = new Event
        {
            DateTime = DateTime.UtcNow,
            EventType = input.EventType,
            EntityType = input.EntityType,
            EntityId = input.EntityId,
            EventData = input.EventData,
        };
        await eventRepository.CreateAsync(eventObject);
    }
}
