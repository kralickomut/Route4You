using Route4You.Application.Abstraction.Ascents;
using Route4You.Application.DataTransfers.Ascents;
using Route4You.Domain.Ascents;

namespace Route4You.Application.Services;

public sealed class AscentService : IAscentService
{
	private readonly IAscentRepository _ascents;

	public AscentService(IAscentRepository ascents)
	{
		this._ascents = ascents;
	}

	public async Task<AscentVm> RecordAsync(RecordAscentDto dto, CancellationToken ct)
	{
		var ascentId = $"{dto.UserId}_{dto.RouteId}";
		var existing = await this._ascents.GetAsync(ascentId, ct);

		Ascent ascent;
		if (existing is null)
		{
			// create new
			ascent = Ascent.Create(
				dto.UserId,
				dto.RouteId,
				dto.DateClimbed,
				dto.Style,
				dto.Rating,
				dto.Notes
			);

			await this._ascents.AddAsync(ascent, ct);
		}
		else
		{
			// update existing
			existing.Update(dto.DateClimbed, dto.Style, dto.Rating, dto.Notes);
			await this._ascents.UpdateAsync(existing, ct);
			ascent = existing;
		}

		return MapToVm(ascent);
	}

	public async Task<IReadOnlyList<AscentVm>> GetByUserAsync(string userId, CancellationToken ct)
	{
		var list = await this._ascents.GetByUserAsync(userId, ct);
		return list.Select(MapToVm).ToList();
	}

	private static AscentVm MapToVm(Ascent a)
	{
		return new AscentVm(
			a.Id,
			a.UserId,
			a.RouteId,
			a.DateClimbed,
			a.Style,
			a.Rating,
			a.Notes,
			a.CreatedAt,
			a.UpdatedAt
		);
	}
}