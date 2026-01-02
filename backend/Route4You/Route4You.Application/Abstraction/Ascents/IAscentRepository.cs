using Route4You.Domain.Ascents;

namespace Route4You.Application.Abstraction.Ascents;

public interface IAscentRepository
{
	Task<Ascent?> GetAsync(string id, CancellationToken ct = default);
	Task AddAsync(Ascent ascent, CancellationToken ct = default);
	Task UpdateAsync(Ascent ascent, CancellationToken ct = default);
	
	Task<IReadOnlyList<Ascent>> GetByUserAsync(string userId, CancellationToken ct = default);
	Task<bool> ExistsForUserAndRouteAsync(string userId, string routeId, CancellationToken ct = default);
}