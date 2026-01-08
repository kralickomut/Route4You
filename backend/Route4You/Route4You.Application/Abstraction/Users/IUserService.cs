using Route4You.Application.DataTransfers.Users;

namespace Route4You.Application.Abstraction.Users;

public interface IUserService
{
	Task<IReadOnlyList<UserVm>> GetAllAsync(CancellationToken ct);
	Task<UserVm?> GetAsync(string id, CancellationToken ct);
}