using Route4You.Domain.Users;

namespace Route4You.Application.Abstraction.Users;

public interface IUserRepository
{
	Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct);
	Task<AppUser?> GetAsync(string id, CancellationToken ct);
}