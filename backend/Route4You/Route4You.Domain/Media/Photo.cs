using MongoDB.Bson.Serialization.Attributes;
using Route4You.Domain.Base;

namespace Route4You.Domain.Media;

[BsonIgnoreExtraElements]
public class Photo : Entity
{
	public string StoragePath { get; set; } = default!; // e.g. "photos/{photoId}.jpg"
	public int Width { get; set; }
	public int Height { get; set; }

	public string UploaderId { get; set; } = default!;

	[BsonIgnoreIfNull] public string? AreaId { get; set; }
	[BsonIgnoreIfNull] public string? RouteId { get; set; }

	[BsonIgnoreIfNull]
	[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
	public DateTime? TakenAt { get; set; }

	// store compact EXIF as string pairs (avoid object blobs)
	public Dictionary<string, string> Exif { get; set; } = new();

	[BsonIgnoreIfNull] public string? DefaultOverlayId { get; set; }

	public static Photo Create(string id, string storagePath, int width, int height, string uploaderId,
		string? areaId, string? routeId, DateTime? takenAt,
		IDictionary<string, string>? exif)
	{
		var now = DateTime.UtcNow;
		return new Photo
		{
			Id = id,
			StoragePath = storagePath,
			Width = width,
			Height = height,
			UploaderId = uploaderId,
			AreaId = areaId,
			RouteId = routeId,
			TakenAt = takenAt,
			Exif = exif is null ? new Dictionary<string, string>() : new Dictionary<string, string>(exif),
			CreatedAt = now,
			UpdatedAt = now
		};
	}
}