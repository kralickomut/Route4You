using Route4You.Application.Abstraction.Users;
using Route4You.Application.DataTransfers.Users;

namespace Route4You.Application.Services;

public sealed class UserService : IUserService
{
	private readonly IUserRepository _repo;

	public UserService(IUserRepository repo)
	{
		this._repo = repo;
	}

	public async Task<IReadOnlyList<UserVm>> GetAllAsync(CancellationToken ct)
	{
		var users = await this._repo.GetAllAsync(ct);
		return users.Select(u => new UserVm(u.Id, u.DisplayName, u.Email)).ToList();
	}

	public async Task<UserVm?> GetAsync(string id, CancellationToken ct)
	{
		var u = await this._repo.GetAsync(id, ct);
		return u is null ? null : new UserVm(u.Id, u.DisplayName, u.Email);
	}
}