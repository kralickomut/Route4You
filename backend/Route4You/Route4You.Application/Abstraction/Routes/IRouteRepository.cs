using Route4You.Domain.Routes;

namespace Route4You.Application.Abstraction.Routes;

public interface IRouteRepository
{
	Task<Route?> GetAsync(string id, CancellationToken ct = default);
	Task AddAsync(Route route, CancellationToken ct = default);
	Task<IReadOnlyList<Route>> GetByAreaAsync(string areaId, CancellationToken ct = default);
}