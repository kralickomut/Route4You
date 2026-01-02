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
}