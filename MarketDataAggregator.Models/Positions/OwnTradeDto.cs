using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MarketDataAggregator.Models.Positions;

public class OwnTradeDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public required DateTime DateTime { get; set; }

    public required string Symbol { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal Price { get; set; }

    public required Direction Direction { get; set; }

    public required decimal Fee { get; set; }

    public required string FeeCurrency { get; set; }

    public required string OrderType { get; set; }

    public required string Strategy { get; set; }
}
