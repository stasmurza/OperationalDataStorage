using OperationalDataStorage.Contracts.Events;
using OperationalDataStorage.Contracts.Ohlcs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OperationalDataStorage.Service.Tests.Factories;

public static class EventsSnapshotFactory
{
    private static readonly JsonSerializerOptions jsonSerializerOptions;

    static EventsSnapshotFactory()
    {
        jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public static EventsSnapshot GenerateEventsSnapshot(IEnumerable<Ohlc> ohlcs)
    {
        return new EventsSnapshot()
        {
            NewEvents = ohlcs.Select(CreateEvent),
        };
    }

    private static EventDto CreateEvent(Ohlc ohlc)
    {
        return new EventDto()
        {
            Id = Guid.NewGuid().ToString(),
            EventId = Guid.NewGuid(),
            EventDateTime = DateTime.UtcNow,
            EventType = EventType.OhlcReceived,
            EventData = JsonSerializer.Serialize(ohlc, jsonSerializerOptions),
            EntityType = EntityType.Ohlc,
            EntityId = Guid.NewGuid().ToString(),
        };
    }
}
