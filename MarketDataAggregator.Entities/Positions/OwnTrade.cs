using MarketDataAggregator.Entities.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MarketDataAggregator.Entities.Positions;

public class OwnTrade : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public required DateTime DateTime { get; set; }

    public required string Symbol { get; set; }

    public required string Strategy { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal Price { get; set; }

    public required Direction Direction { get; set; }

    public required decimal Fee { get; set; }

    public required string FeeCurrency { get; set; }

    public required string OrderType { get; set; }
}
