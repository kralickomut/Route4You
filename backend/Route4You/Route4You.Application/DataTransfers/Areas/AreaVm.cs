using Route4You.Domain.Areas;

namespace Route4You.Application.DataTransfers.Areas;

public record AreaVm(
    string Id,
    string Name,
    AreaType Type,
    string? ParentId,
    List<string> PathIds,
    List<string> PathNames,
    DateTime CreatedAt,
    DateTime UpdatedAt
);