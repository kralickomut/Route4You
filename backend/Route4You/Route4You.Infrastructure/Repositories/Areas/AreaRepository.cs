using MongoDB.Driver;
using Route4You.Application.Abstraction.Areas;
using Route4You.Domain.Areas;
using Route4You.Infrastructure.Data;

namespace Route4You.Infrastructure.Repositories.Areas;

public sealed class AreaRepository : IAreaRepository
{
	private readonly IMongoCollection<Area> _col;

	public AreaRepository(MongoContext ctx)
	{
		this._col = ctx.GetCollection<Area>("areas");
	}

	public Task<Area?> GetAsync(string id, CancellationToken ct = default)
	{
		return this._col.Find(a => a.Id == id).FirstOrDefaultAsync(ct)!;
	}

	public Task AddAsync(Area area, CancellationToken ct = default)
	{
		return this._col.InsertOneAsync(area, cancellationToken: ct);
	}

	public async Task<IReadOnlyList<Area>> GetByParentAsync(string? parentId, CancellationToken ct)
	{
		// For root areas, ParentId == null
		FilterDefinition<Area> filter;

		if (string.IsNullOrEmpty(parentId))
			// fetch top-level areas (ParentId is null or missing)
			filter = Builders<Area>.Filter.Eq(a => a.ParentId, null);
		else
			filter = Builders<Area>.Filter.Eq(a => a.ParentId, parentId);

		var result = await this._col.Find(filter).ToListAsync(ct);
		return result;
	}

	public Task IncrementChildrenCountAsync(string parentId, CancellationToken ct)
	{
		var update = Builders<Area>.Update
			.Inc(a => a.ChildrenCount, 1)
			.Set(a => a.UpdatedAt, DateTime.UtcNow);

		return this._col.UpdateOneAsync(a => a.Id == parentId, update, cancellationToken: ct);
	}

	public Task IncrementRoutesCountAsync(string areaId, CancellationToken ct)
	{
		var update = Builders<Area>.Update
			.Inc(a => a.RoutesCount, 1)
			.Set(a => a.UpdatedAt, DateTime.UtcNow);

		return this._col.UpdateOneAsync(a => a.Id == areaId, update, cancellationToken: ct);
	}

	public Task IncrementRoutesCountManyAsync(IEnumerable<string> areaIds, CancellationToken ct)
	{
		var filter = Builders<Area>.Filter.In(a => a.Id, areaIds);
		var update = Builders<Area>.Update
			.Inc(a => a.RoutesCount, 1)
			.Set(a => a.UpdatedAt, DateTime.UtcNow);

		return this._col.UpdateManyAsync(filter, update, cancellationToken: ct);
	}

	public Task UpdateFieldsAsync(string id, string name, AreaType type, double? lat, double? lng, CancellationToken ct)
	{
		var update = Builders<Area>.Update
			.Set(a => a.Name, name)
			.Set(a => a.Type, type)
			.Set(a => a.Latitude, lat)
			.Set(a => a.Longitude, lng)
			.Set(a => a.UpdatedAt, DateTime.UtcNow);

		return this._col.UpdateOneAsync(a => a.Id == id, update, cancellationToken: ct);
	}

	public async Task<IReadOnlyList<string>> GetSubtreeAreaIdsAsync(string rootAreaId, CancellationToken ct)
	{
		// root + potomci: PathIds obsahuje rootAreaId
		var filter = Builders<Area>.Filter.Or(
			Builders<Area>.Filter.Eq(a => a.Id, rootAreaId),
			Builders<Area>.Filter.AnyEq(a => a.PathIds, rootAreaId)
		);

		var ids = await this._col.Find(filter)
			.Project(a => a.Id)
			.ToListAsync(ct);

		return ids;
	}

	public Task DeleteManyAsync(IEnumerable<string> areaIds, CancellationToken ct)
	{
		var filter = Builders<Area>.Filter.In(a => a.Id, areaIds);
		return this._col.DeleteManyAsync(filter, ct);
	}

	public Task DecrementRoutesCountManyAsync(IEnumerable<string> areaIds, CancellationToken ct)
	{
		var filter = Builders<Area>.Filter.In(a => a.Id, areaIds);
		var update = Builders<Area>.Update
			.Inc(a => a.RoutesCount, -1)
			.Set(a => a.UpdatedAt, DateTime.UtcNow);

		return this._col.UpdateManyAsync(filter, update, cancellationToken: ct);
	}

	public async Task<IReadOnlyList<Area>> GetAllAsync(CancellationToken ct)
	{
		return await this._col.Find(Builders<Area>.Filter.Empty).ToListAsync(ct);
	}

	public Task SetCountsAsync(string areaId, int childrenCount, int routesCount, CancellationToken ct)
	{
		var update = Builders<Area>.Update
			.Set(a => a.ChildrenCount, childrenCount)
			.Set(a => a.RoutesCount, routesCount)
			.Set(a => a.UpdatedAt, DateTime.UtcNow);

		return this._col.UpdateOneAsync(a => a.Id == areaId, update, cancellationToken: ct);
	}
}