using Route4You.Application.DataTransfers.Routes;

namespace Route4You.Application.Abstraction.Routes;

public interface IRouteService
{
	Task<RouteVm> CreateAsync(CreateRouteDto dto, CancellationToken ct);
	Task<RouteVm?> GetAsync(string id, CancellationToken ct);
	Task<IReadOnlyList<RouteVm>> GetByAreaAsync(string areaId, CancellationToken ct);
}