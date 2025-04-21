using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using OperationalDataStorage.Domain.Entities.Abstractions;

namespace OperationalDataStorage.Domain.Entities.Ohlcs;

public class Ohlc : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Start of the interval time.
    /// </summary>
    [BsonDateTimeOptions(Representation = BsonType.Document)]
    public required DateTime StartTime { get; set; }

    /// <summary>
    /// Time of the last trade.
    /// </summary>
    [BsonDateTimeOptions(Representation = BsonType.Document)]
    public required DateTime EndTime { get; set; }

    /// <summary>
    /// Symbol.
    /// </summary>
    public required string Symbol { get; set; }

    /// <summary>
    /// Interval.
    /// </summary>
    public required TimeInterval Interval { get; set; }

    public required decimal Low { get; set; }

    public required decimal High { get; set; }

    public required decimal Open { get; set; }

    public required decimal Close { get; set; }

    public required decimal Volume { get; set; }
}
