using OperationalDataStorage.Domain.Entities;
using OperationalDataStorage.Domain.Entities.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OperationalDataStorage.Domain.Entities.Positions;

public class Order : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public required Guid OrderId { get; set; }
}
