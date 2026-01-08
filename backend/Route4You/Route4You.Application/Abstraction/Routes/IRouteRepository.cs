using Route4You.Domain.Routes;

namespace Route4You.Application.Abstraction.Routes;

public interface IRouteRepository
{
	Task<Route?> GetAsync(string id, CancellationToken ct = default);
	Task AddAsync(Route route, CancellationToken ct = default);
	Task<IReadOnlyList<Route>> GetByAreaAsync(string areaId, CancellationToken ct = default);
	
	Task IncrementAscentCountAsync(string routeId, CancellationToken ct = default);
	Task SetRatingStatsAsync(string routeId, double? ratingAvg, int ratingsCount, CancellationToken ct);

	Task UpdateFieldsAsync(
		string id,
		string name,
		string grade,
		int pitches,
		int? lengthMeters,
		RouteStyle style,
		List<string> tags,
		string? defaultPhotoId,
		CancellationToken ct
	);
	
	Task<IReadOnlyList<string>> GetRouteIdsByAreaIdsAsync(IReadOnlyList<string> areaIds, CancellationToken ct);
	Task DeleteByAreaIdsAsync(IReadOnlyList<string> areaIds, CancellationToken ct);
	
	Task DeleteAsync(string id, CancellationToken ct = default);
	
	Task<IReadOnlyList<Route>> GetAllAsync(CancellationToken ct);
}