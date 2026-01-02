using Route4You.Domain.Ascents;

namespace Route4You.Application.DataTransfers.Ascents;

public sealed record AscentVm(
	string Id,
	string UserId,
	string RouteId,
	DateTime? DateClimbed,
	AscentStyle Style,
	int? Rating,
	string? Notes,
	DateTime CreatedAt,
	DateTime UpdatedAt
);