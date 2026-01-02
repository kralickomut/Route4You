using Route4You.Application.Abstraction.Areas;
using Route4You.Application.Abstraction.Routes;
using Route4You.Application.DataTransfers.Routes;
using Route4You.Domain.Routes;

namespace Route4You.Application.Services;

public sealed class RouteService : IRouteService
{
	private readonly IRouteRepository _routes;
	private readonly IAreaRepository _areas;

	public RouteService(IRouteRepository routes, IAreaRepository areas)
	{
		this._routes = routes;
		this._areas = areas;
	}

	public async Task<RouteVm> CreateAsync(CreateRouteDto dto, CancellationToken ct)
	{
		var area = await this._areas.GetAsync(dto.AreaId, ct)
		           ?? throw new InvalidOperationException($"Area '{dto.AreaId}' not found.");

		// Breadcrumb path (area path + area itself + route name optionally)
		var pathNames = area.PathNames.Append(area.Name).ToList();

		var route = Route.Create(
			Guid.NewGuid().ToString(),
			dto.Name,
			dto.Grade,
			dto.AreaId,
			pathNames,
			dto.Pitches,
			dto.LengthMeters,
			dto.Style,
			dto.Tags ?? new List<string>(),
			dto.CreatedByUserId
		);

		await this._routes.AddAsync(route, ct);

		return MapToVm(route);
	}

	public async Task<RouteVm?> GetAsync(string id, CancellationToken ct)
	{
		var r = await this._routes.GetAsync(id, ct);
		return r is null ? null : MapToVm(r);
	}

	public async Task<IReadOnlyList<RouteVm>> GetByAreaAsync(string areaId, CancellationToken ct)
	{
		var list = await this._routes.GetByAreaAsync(areaId, ct);
		return list.Select(MapToVm).ToList();
	}

	private static RouteVm MapToVm(Route r)
	{
		return new RouteVm(
			r.Id,
			r.Name,
			r.Grade,
			r.AreaId,
			r.PathNames,
			r.Pitches,
			r.LengthMeters,
			r.Style,
			r.Tags,
			r.CreatedByUserId,
			r.AscentsCount,
			r.RatingAvg,
			r.RatingsCount,
			r.CreatedAt,
			r.UpdatedAt
		);
	}
}