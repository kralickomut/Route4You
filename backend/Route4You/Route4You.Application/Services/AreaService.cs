using Route4You.Application.Abstraction.Areas;
using Route4You.Application.Abstraction.Ascents;
using Route4You.Application.Abstraction.Routes;
using Route4You.Application.DataTransfers.Areas;
using Route4You.Domain.Areas;

namespace Route4You.Application.Services;

public sealed class AreaService : IAreaService
{
	private readonly IAreaRepository _repo;
	private readonly IRouteRepository _routes;
	private readonly IAscentRepository _ascents;

	public AreaService(IAreaRepository repo, IRouteRepository routes, IAscentRepository ascents)
	{
		this._repo = repo;
		this._routes = routes;
		this._ascents = ascents;
	}

	public async Task<AreaVm> CreateAsync(CreateAreaDto dto, CancellationToken ct)
	{
		// Optional: derive path info from parent if provided
		List<string> pathIds = new();
		List<string> pathNames = new();

		if (!string.IsNullOrWhiteSpace(dto.ParentId))
		{
			var parent = await this._repo.GetAsync(dto.ParentId!, ct)
			             ?? throw new InvalidOperationException($"Parent area '{dto.ParentId}' not found.");

			pathIds = parent.PathIds.Append(parent.Id).ToList();
			pathNames = parent.PathNames.Append(parent.Name).ToList();
		}

		var slug = dto.Name.ToLower().Replace(" ", "-");

		var area = Area.Create(
			dto.Name,
			dto.Type,
			dto.ParentId,
			pathIds,
			pathNames,
			slug,
			dto.Latitude,
			dto.Longtitude
		);

		await this._repo.AddAsync(area, ct);

		if (!string.IsNullOrWhiteSpace(dto.ParentId)) await this._repo.IncrementChildrenCountAsync(dto.ParentId!, ct);

		return MapToVm(area);
	}

	public async Task<AreaVm?> GetAsync(string id, CancellationToken ct)
	{
		var a = await this._repo.GetAsync(id, ct);
		return a is null ? null : MapToVm(a);
	}

	public async Task<IReadOnlyList<AreaVm>> GetChildrenAsync(string? parentId, CancellationToken ct)
	{
		var children = await this._repo.GetByParentAsync(parentId, ct);
		return children.Select(MapToVm).ToList();
	}

	public async Task<AreaVm> UpdateAsync(UpdateAreaDto dto, CancellationToken ct)
	{
		var existing = await this._repo.GetAsync(dto.Id, ct)
		               ?? throw new InvalidOperationException($"Area '{dto.Id}' not found.");

		//  slug update from name
		var slug = dto.Name.ToLowerInvariant().Replace(" ", "-");

		await this._repo.UpdateFieldsAsync(
			dto.Id,
			dto.Name,
			dto.Type,
			dto.Latitude,
			dto.Longitude,
			ct
		);

		var updated = await this._repo.GetAsync(dto.Id, ct) ?? existing;
		updated.Slug = slug;

		return MapToVm(updated);
	}

	public async Task DeleteRecursiveAsync(string id, CancellationToken ct)
	{
		// 1) zjisti všechny areaIds, které patří do subtree (id + všichni potomci)
		var subtreeAreaIds = await this._repo.GetSubtreeAreaIdsAsync(id, ct);
		// pokud neexistuje ani root area, klidně skonči (nebo throw)
		if (subtreeAreaIds.Count == 0) return;

		// 2) najdi routes v těchto areas (jen idčka)
		var routeIds = await this._routes.GetRouteIdsByAreaIdsAsync(subtreeAreaIds, ct);

		// 3) smaž ascents pro ty routes
		if (routeIds.Count > 0)
			await this._ascents.DeleteByRouteIdsAsync(routeIds, ct);

		// 4) smaž routes
		await this._routes.DeleteByAreaIdsAsync(subtreeAreaIds, ct);

		// 5) smaž všechny areas ze subtree (včetně root)
		await this._repo.DeleteManyAsync(subtreeAreaIds, ct);
	}

	public async Task RebuildCountsAsync(CancellationToken ct)
	{
		// 1) načti vše
		var areas = await _repo.GetAllAsync(ct);
		var routes = await _routes.GetAllAsync(ct);

		// 2) spočítej childrenCount podle ParentId
		var childrenByParent = areas
			.Where(a => !string.IsNullOrWhiteSpace(a.ParentId))
			.GroupBy(a => a.ParentId!)
			.ToDictionary(g => g.Key, g => g.Count());

		// 3) spočítej routesCount podle AreaId
		var routesByArea = routes
			.GroupBy(r => r.AreaId)
			.ToDictionary(g => g.Key, g => g.Count());

		// 4) nastav hodnoty všem oblastem (bulk update)
		foreach (var a in areas)
		{
			var cc = childrenByParent.TryGetValue(a.Id, out var c) ? c : 0;
			var rc = routesByArea.TryGetValue(a.Id, out var r) ? r : 0;

			await _repo.SetCountsAsync(a.Id, cc, rc, ct);
		}
	}

	private static AreaVm MapToVm(Area a)
	{
		return new AreaVm(
			a.Id,
			a.Name,
			a.Type,
			a.ParentId,
			a.PathIds,
			a.PathNames,
			a.ChildrenCount,
			a.RoutesCount,
			a.CreatedAt,
			a.UpdatedAt
		);
	}
}