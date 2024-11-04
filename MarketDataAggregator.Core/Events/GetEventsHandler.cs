using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Models.Entities.Events;
using MarketDataAggregator.Models.Events;
using MediatR;

namespace MarketDataAggregator.Core.Events;

public class GetEventsHandler(IRepository<Event> eventRepository) : IRequestHandler<GetEventsInput, GetEventsOutput>
{
    private readonly IRepository<Event> eventRepository = eventRepository;

    public async Task<GetEventsOutput> Handle(GetEventsInput input, CancellationToken cancellationToken)
    {
        var eventsById = new Dictionary<string, Event>();

        var events = (input.EventType, input.EntityType) switch
        {
            (null, null) => await GetEvents(input.Start, input.End),
            (null, EntityType entityType) => await GetEvents(input.Start, input.End, entityType),
            (EventType eventType, null) => await GetEvents(input.Start, input.End, eventType),
            (EventType eventType, EntityType entityType) => await GetEvents(input.Start, input.End, eventType, entityType),
        };

        return new GetEventsOutput()
        {
            Events = events.Select(i => new EventDto
            {
                Id = i.Id,
                DateTime = i.DateTime,
                EventType = i.EventType,
                EntityType = i.EntityType,
                EntityId = i.EntityId,
                EventData = i.EventData,
            }),
        };
    }

    private Task<IEnumerable<Event>> GetEvents(DateTime start, DateTime end)
    {
        return eventRepository.GetAsync(i =>
            i.DateTime >= start &&
            i.DateTime <= end);
    }

    private Task<IEnumerable<Event>> GetEvents(DateTime start, DateTime end, EventType eventType)
    {
        return eventRepository.GetAsync(i =>
            i.DateTime >= start &&
            i.DateTime <= end &&
            i.EventType == eventType.ToString());
    }

    private Task<IEnumerable<Event>> GetEvents(DateTime start, DateTime end, EntityType entityType)
    {
        return eventRepository.GetAsync(i =>
            i.DateTime >= start &&
            i.DateTime <= end &&
            i.EntityType == entityType.ToString());
    }

    private Task<IEnumerable<Event>> GetEvents(DateTime start, DateTime end, EventType eventType, EntityType entityType)
    {
        return eventRepository.GetAsync(i =>
            i.DateTime >= start &&
            i.DateTime <= end &&
            i.EventType == eventType.ToString() &&
            i.EntityType == entityType.ToString());
    }
}