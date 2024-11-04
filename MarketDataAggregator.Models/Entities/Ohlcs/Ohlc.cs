using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MarketDataAggregator.Models.Entities.Abstractions;

namespace MarketDataAggregator.Models.Entities.Ohlcs;

public class Ohlc : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public required DateTime StartTime { get; set; }

    public required string Symbol { get; set; }

    public required TimeInterval Interval { get; set; }

    public required DateTime MinTime { get; set; }

    public required DateTime MaxTime { get; set; }

    public required decimal Low { get; set; }

    public required decimal High { get; set; }

    public required decimal Open { get; set; }

    public required decimal Close { get; set; }

    public required decimal Volume { get; set; }
}
