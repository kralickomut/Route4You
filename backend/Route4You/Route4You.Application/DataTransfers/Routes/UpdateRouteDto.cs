using Route4You.Domain.Routes;

namespace Route4You.Application.DataTransfers.Routes;

public sealed record UpdateRouteDto(
	string Id,
	string Name,
	string Grade,
	int Pitches,
	int? LengthMeters,
	RouteStyle Style,
	List<string>? Tags,
	string? DefaultPhotoId
);