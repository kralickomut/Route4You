using MongoDB.Bson.Serialization.Attributes;

namespace Route4You.Domain.Base;

public abstract class Entity
{
    [BsonId] public string Id { get; set; } = default!;
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    protected Entity() {}

}