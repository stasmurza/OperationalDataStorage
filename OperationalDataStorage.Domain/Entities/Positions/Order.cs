using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace OperationalDataStorage.Domain.Entities.Positions;

public class Order
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public required Guid OrderId { get; set; }
}
