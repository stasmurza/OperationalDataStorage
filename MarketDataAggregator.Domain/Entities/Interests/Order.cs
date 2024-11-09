using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MarketDataAggregator.Domain.Entities;

namespace MarketDataAggregator.Domain.Entities.Interests;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public required string Symbol { get; set; }

    public required string Strategy { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal Price { get; set; }

    public required Direction Direction { get; set; }
}
