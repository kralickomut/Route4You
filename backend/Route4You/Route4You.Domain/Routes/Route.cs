using MongoDB.Bson.Serialization.Attributes;
using Route4You.Domain.Base;

namespace Route4You.Domain.Routes;

[BsonIgnoreExtraElements]
public class Route : Entity
{
	public string Name { get; set; } = default!;
	public string Grade { get; set; } = default!; // e.g., "6c"

	public string AreaId { get; set; } = default!;
	public List<string> PathNames { get; set; } = new(); // ["Czech Republic","Moravský Kras","Tančící skála","Rohová"]

	public int Pitches { get; set; } = 1;
	[BsonIgnoreIfNull] public int? LengthMeters { get; set; }
	public RouteStyle Style { get; set; } = RouteStyle.Sport;

	public List<string> Tags { get; set; } = new();

	public string CreatedByUserId { get; set; } = default!;
	[BsonIgnoreIfNull] public string? DefaultPhotoId { get; set; }

	// denormalized counters
	public int AscentsCount { get; set; }
	[BsonIgnoreIfNull] public double? RatingAvg { get; set; }
	public int RatingsCount { get; set; }

	public static Route Create(string id, string name, string grade, string areaId,
		IEnumerable<string> pathNames, int pitches, int? lengthMeters,
		RouteStyle style, IEnumerable<string> tags, string createdByUserId)
	{
		var now = DateTime.UtcNow;
		return new Route
		{
			Id = id,
			Name = name,
			Grade = grade,
			AreaId = areaId,
			PathNames = pathNames?.ToList() ?? new List<string>(),
			Pitches = Math.Max(1, pitches),
			LengthMeters = lengthMeters,
			Style = style,
			Tags = tags?.ToList() ?? new List<string>(),
			CreatedByUserId = createdByUserId,
			CreatedAt = now,
			UpdatedAt = now
		};
	}

	public void IncrementAscents()
	{
		this.AscentsCount++;
		this.UpdatedAt = DateTime.UtcNow;
	}

	public void ApplyNewRating(int stars) // 1..5
	{
		stars = Math.Max(1, Math.Min(5, stars));
		var total = (this.RatingAvg ?? 0d) * this.RatingsCount + stars;
		this.RatingsCount++;
		this.RatingAvg = total / this.RatingsCount;
		this.UpdatedAt = DateTime.UtcNow;
	}
}