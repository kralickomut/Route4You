using MongoDB.Bson.Serialization.Attributes;
using Route4You.Domain.Base;

namespace Route4You.Domain.Users;

[BsonIgnoreExtraElements]
public class AppUser : Entity
{
	public string DisplayName { get; set; } = default!;
	public string Email { get; set; } = default!;
	[BsonIgnoreIfNull] public string? PhotoUrl { get; set; }
	[BsonIgnoreIfNull] public string? HomeCountry { get; set; }

	public int AscentsCount { get; set; }
	public int RoutesCreated { get; set; }

	public static AppUser Create(string id, string displayName, string email, string? photoUrl, string? homeCountry)
	{
		var now = DateTime.UtcNow;
		return new AppUser
		{
			Id = id,
			DisplayName = displayName,
			Email = email,
			PhotoUrl = photoUrl,
			HomeCountry = homeCountry,
			CreatedAt = now,
			UpdatedAt = now
		};
	}

	public void IncrementAscents()
	{
		this.AscentsCount++;
		this.UpdatedAt = DateTime.UtcNow;
	}

	public void IncrementRoutesCreated()
	{
		this.RoutesCreated++;
		this.UpdatedAt = DateTime.UtcNow;
	}
}