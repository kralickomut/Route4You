using MongoDB.Bson.Serialization.Attributes;
using Route4You.Domain.Base;

namespace Route4You.Domain.Areas;

[BsonIgnoreExtraElements]
public class Area : Entity
{
    public string Name { get; set; } = default!;
    public AreaType Type { get; set; }
    
    [BsonIgnoreIfNull]
    public string? ParentId { get; set; }
    
    public List<string> PathIds { get; set; } = new();
    public List<string> PathNames { get; set; } = new();
    
    public string Slug { get; set; } = default!;
    
    [BsonIgnoreIfNull] public double? Latitude { get; set; }
    [BsonIgnoreIfNull] public double? Longitude { get; set; }
    
    public int ChildrenCount { get; set; }
    public int RoutesCount { get; set; }
    
    public Area() { }

    public static Area Create(string name, AreaType type, string? parentId,
        IEnumerable<string>? pathIds, IEnumerable<string>? pathNames,
        string slug, double? lat, double? lng)
    {
        var now = DateTime.UtcNow;
        return new Area
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Type = type,
            ParentId = parentId,
            PathIds = pathIds?.ToList() ?? new(),
            PathNames = pathNames?.ToList() ?? new(),
            Slug = slug ?? string.Empty,
            Latitude = lat,
            Longitude = lng,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}