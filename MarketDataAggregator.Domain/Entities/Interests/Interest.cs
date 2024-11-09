using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MarketDataAggregator.Domain.Entities;
using MarketDataAggregator.Domain.Entities.Abstractions;

namespace MarketDataAggregator.Domain.Entities.Interests;

public class Interest : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public required string Symbol { get; set; }

    public required string Strategy { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal AveragePrice { get; set; }

    public required Direction Direction { get; set; }

    public required List<Order> Orders { get; set; }
}
