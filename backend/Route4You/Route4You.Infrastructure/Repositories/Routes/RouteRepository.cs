using MongoDB.Driver;
using Route4You.Application.Abstraction.Routes;
using Route4You.Domain.Routes;
using Route4You.Infrastructure.Data;

namespace Route4You.Infrastructure.Repositories.Routes;

public sealed class RouteRepository : IRouteRepository
{
	private readonly IMongoCollection<Route> _col;

	public RouteRepository(MongoContext ctx)
	{
		this._col = ctx.GetCollection<Route>("routes");
	}

	public async Task<IReadOnlyList<Route>> GetAllAsync(CancellationToken ct)
	{
		return await this._col.Find(Builders<Route>.Filter.Empty).ToListAsync(ct);
	}

	public Task<Route?> GetAsync(string id, CancellationToken ct = default)
	{
		return this._col.Find(r => r.Id == id).FirstOrDefaultAsync(ct)!;
	}

	public Task AddAsync(Route route, CancellationToken ct = default)
	{
		return this._col.InsertOneAsync(route, cancellationToken: ct);
	}

	public async Task<IReadOnlyList<Route>> GetByAreaAsync(string areaId, CancellationToken ct = default)
	{
		var filter = Builders<Route>.Filter.Eq(r => r.AreaId, areaId);
		return await this._col.Find(filter).SortBy(r => r.Name).ToListAsync(ct);
	}

	public Task IncrementAscentCountAsync(string routeId, CancellationToken ct = default)
	{
		var filter = Builders<Route>.Filter.Eq(r => r.Id, routeId);
		var update = Builders<Route>.Update.Inc(r => r.AscentsCount, 1);
		return this._col.UpdateOneAsync(filter, update, cancellationToken: ct);
	}

	public Task SetRatingStatsAsync(string routeId, double? ratingAvg, int ratingsCount, CancellationToken ct)
	{
		var update = Builders<Route>.Update
			.Set(r => r.RatingAvg, ratingAvg)
			.Set(r => r.RatingsCount, ratingsCount)
			.Set(r => r.UpdatedAt, DateTime.UtcNow);

		return this._col.UpdateOneAsync(r => r.Id == routeId, update, cancellationToken: ct);
	}

	public Task UpdateFieldsAsync(
		string id,
		string name,
		string grade,
		int pitches,
		int? lengthMeters,
		RouteStyle style,
		List<string> tags,
		string? defaultPhotoId,
		CancellationToken ct)
	{
		var update = Builders<Route>.Update
			.Set(r => r.Name, name)
			.Set(r => r.Grade, grade)
			.Set(r => r.Pitches, Math.Max(1, pitches))
			.Set(r => r.LengthMeters, lengthMeters)
			.Set(r => r.Style, style)
			.Set(r => r.Tags, tags ?? new List<string>())
			.Set(r => r.DefaultPhotoId, defaultPhotoId)
			.Set(r => r.UpdatedAt, DateTime.UtcNow);

		return this._col.UpdateOneAsync(r => r.Id == id, update, cancellationToken: ct);
	}

	public async Task<IReadOnlyList<string>> GetRouteIdsByAreaIdsAsync(IReadOnlyList<string> areaIds,
		CancellationToken ct)
	{
		var filter = Builders<Route>.Filter.In(r => r.AreaId, areaIds);

		var ids = await this._col.Find(filter)
			.Project(r => r.Id)
			.ToListAsync(ct);

		return ids;
	}

	public Task DeleteByAreaIdsAsync(IReadOnlyList<string> areaIds, CancellationToken ct)
	{
		var filter = Builders<Route>.Filter.In(r => r.AreaId, areaIds);
		return this._col.DeleteManyAsync(filter, ct);
	}

	public Task DeleteAsync(string id, CancellationToken ct = default)
	{
		return this._col.DeleteOneAsync(r => r.Id == id, ct);
	}
}