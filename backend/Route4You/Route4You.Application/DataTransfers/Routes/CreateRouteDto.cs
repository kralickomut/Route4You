using Route4You.Domain.Routes;

namespace Route4You.Application.DataTransfers.Routes;

public sealed record CreateRouteDto(
	string Name,
	string Grade,
	string AreaId,
	int Pitches,
	int? LengthMeters,
	RouteStyle Style,
	List<string> Tags,
	string CreatedByUserId
);