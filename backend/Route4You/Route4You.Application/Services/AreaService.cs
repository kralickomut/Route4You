using Route4You.Application.Abstraction.Areas;
using Route4You.Application.DataTransfers.Areas;
using Route4You.Domain.Areas;

namespace Route4You.Application.Services;

public sealed class AreaService : IAreaService
{
	private readonly IAreaRepository _repo;

	public AreaService(IAreaRepository repo)
	{
		this._repo = repo;
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

	private static AreaVm MapToVm(Area a)
	{
		return new AreaVm(
			a.Id,
			a.Name,
			a.Type,
			a.ParentId,
			a.PathIds,
			a.PathNames,
			a.CreatedAt,
			a.UpdatedAt
		);
	}
}