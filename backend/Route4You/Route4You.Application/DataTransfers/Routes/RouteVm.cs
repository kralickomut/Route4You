using Route4You.Domain.Routes;

namespace Route4You.Application.DataTransfers.Routes;

public sealed record RouteVm(
	string Id,
	string Name,
	string Grade,
	string AreaId,
	List<string> PathNames,
	int Pitches,
	int? LengthMeters,
	RouteStyle Style,
	List<string> Tags,
	string CreatedByUserId,
	int AscentsCount,
	double? RatingAvg,
	int RatingsCount,
	DateTime CreatedAt,
	DateTime UpdatedAt
);