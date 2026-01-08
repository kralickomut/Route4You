using MongoDB.Driver;
using Route4You.Application.Abstraction.Users;
using Route4You.Domain.Users;
using Route4You.Infrastructure.Data;

namespace Route4You.Infrastructure.Repositories.Users;

public sealed class UserRepository : IUserRepository
{
	private readonly IMongoCollection<AppUser> _col;

	public UserRepository(MongoContext ctx)
	{
		this._col = ctx.GetCollection<AppUser>("users");
	}

	public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct)
	{
		return await this._col.Find(_ => true).SortBy(u => u.DisplayName).ToListAsync(ct);
	}

	public Task<AppUser?> GetAsync(string id, CancellationToken ct)
	{
		return this._col.Find(u => u.Id == id).FirstOrDefaultAsync(ct)!;
	}
}