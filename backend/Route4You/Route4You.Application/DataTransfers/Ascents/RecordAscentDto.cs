using Route4You.Domain.Ascents;

namespace Route4You.Application.DataTransfers.Ascents;

public sealed record RecordAscentDto(
	string UserId,
	string RouteId,
	DateTime? DateClimbed,
	AscentStyle Style,
	int? Rating,
	string? Notes
);