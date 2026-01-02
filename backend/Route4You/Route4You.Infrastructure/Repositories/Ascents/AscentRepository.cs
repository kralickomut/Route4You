using MongoDB.Driver;
using Route4You.Application.Abstraction.Ascents;
using Route4You.Domain.Ascents;
using Route4You.Infrastructure.Data;

namespace Route4You.Infrastructure.Repositories.Ascents;

public sealed class AscentRepository : IAscentRepository
{
	private readonly IMongoCollection<Ascent> _col;

	public AscentRepository(MongoContext ctx)
	{
		this._col = ctx.GetCollection<Ascent>("ascents");
	}

	public Task<Ascent?> GetAsync(string id, CancellationToken ct = default)
	{
		return this._col.Find(a => a.Id == id).FirstOrDefaultAsync(ct)!;
	}

	public Task AddAsync(Ascent ascent, CancellationToken ct = default)
	{
		return this._col.InsertOneAsync(ascent, cancellationToken: ct);
	}

	public Task UpdateAsync(Ascent ascent, CancellationToken ct = default)
	{
		return this._col.ReplaceOneAsync(
			x => x.Id == ascent.Id,
			ascent,
			cancellationToken: ct
		);
	}

	public async Task<IReadOnlyList<Ascent>> GetByUserAsync(string userId, CancellationToken ct = default)
	{
		var filter = Builders<Ascent>.Filter.Eq(a => a.UserId, userId);
		var result = await this._col.Find(filter)
			.SortByDescending(a => a.DateClimbed)
			.ToListAsync(ct);

		return result;
	}

	public async Task<bool> ExistsForUserAndRouteAsync(string userId, string routeId, CancellationToken ct = default)
	{
		var filter = Builders<Ascent>.Filter.And(
			Builders<Ascent>.Filter.Eq(a => a.UserId, userId),
			Builders<Ascent>.Filter.Eq(a => a.RouteId, routeId)
		);

		return await this._col.Find(filter).AnyAsync(ct);
	}
}