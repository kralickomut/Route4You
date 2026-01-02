using MongoDB.Bson.Serialization.Attributes;
using Route4You.Domain.Base;

namespace Route4You.Domain.Ascents;

[BsonIgnoreExtraElements]
public class Ascent : Entity
{
	public string UserId { get; set; } = default!;
	public string RouteId { get; set; } = default!;

	[BsonIgnoreIfNull]
	[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
	public DateTime? DateClimbed { get; set; }

	public AscentStyle Style { get; set; } = AscentStyle.Redpoint;

	[BsonIgnoreIfNull] public int? Rating { get; set; } // 1..5
	[BsonIgnoreIfNull] public string? Notes { get; set; }

	// Id recommendation: $"{userId}_{routeId}" to guarantee uniqueness per pair
	public static Ascent Create(string userId, string routeId, DateTime? dateClimbed, AscentStyle style, int? rating,
		string? notes)
	{
		var now = DateTime.UtcNow;
		return new Ascent
		{
			Id = $"{userId}_{routeId}",
			UserId = userId,
			RouteId = routeId,
			DateClimbed = dateClimbed,
			Style = style,
			Rating = rating,
			Notes = notes,
			CreatedAt = now,
			UpdatedAt = now
		};
	}

	public void Update(DateTime? dateClimbed, AscentStyle style, int? rating, string? notes)
	{
		this.DateClimbed = dateClimbed;
		this.Style = style;
		this.Rating = rating;
		this.Notes = notes;
		this.UpdatedAt = DateTime.UtcNow;
	}
}