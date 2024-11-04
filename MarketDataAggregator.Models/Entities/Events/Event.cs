using MarketDataAggregator.Models.Entities.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MarketDataAggregator.Models.Entities.Events;

public class Event : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public required DateTime DateTime { get; set; }

    public required string EventType { get; set; }

    public required string EntityType { get; set; }

    public required string EntityId { get; set; }

    public required string EventData { get; set; }

    public bool Processed { get; set; } = false;
}
