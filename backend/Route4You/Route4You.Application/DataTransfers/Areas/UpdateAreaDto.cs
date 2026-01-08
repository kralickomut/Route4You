using Route4You.Domain.Areas;

namespace Route4You.Application.DataTransfers.Areas;

public sealed record UpdateAreaDto(
	string Id,
	string Name,
	AreaType Type,
	double? Latitude,
	double? Longitude
);