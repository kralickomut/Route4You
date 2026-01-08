using Route4You.Domain.Areas;

namespace Route4You.Application.DataTransfers.Areas;

public sealed record AreaVm(
	string Id,
	string Name,
	AreaType Type,
	string? ParentId,
	List<string> PathIds,
	List<string> PathNames,
	int ChildrenCount,
	int RoutesCount,
	DateTime CreatedAt,
	DateTime UpdatedAt
);

