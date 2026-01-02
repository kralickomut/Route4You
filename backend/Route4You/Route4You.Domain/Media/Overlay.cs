using MongoDB.Bson.Serialization.Attributes;
using Route4You.Domain.Base;

namespace Route4You.Domain.Media;


[BsonIgnoreExtraElements]
public class Overlay : Entity
{
    public string PhotoId { get; set; } = default!;
    public string RouteId { get; set; } = default!;
    public string CreatedByUserId { get; set; } = default!;

    public OverlayGeometry Geometry { get; set; } = OverlayGeometry.Empty;

    [BsonIgnoreIfNull] public string? ColorHex { get; set; }

    public Overlay() { }

    public static Overlay Create(string id, string photoId, string routeId, string createdByUserId,
        OverlayGeometry geometry, string? colorHex)
    {
        var now = DateTime.UtcNow;
        return new Overlay
        {
            Id = id,
            PhotoId = photoId,
            RouteId = routeId,
            CreatedByUserId = createdByUserId,
            Geometry = geometry ?? OverlayGeometry.Empty,
            ColorHex = colorHex,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}