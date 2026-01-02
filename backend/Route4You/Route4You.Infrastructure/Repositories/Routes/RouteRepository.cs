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
}