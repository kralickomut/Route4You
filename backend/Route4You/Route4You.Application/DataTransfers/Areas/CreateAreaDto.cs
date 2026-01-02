using Route4You.Domain.Areas;

namespace Route4You.Application.DataTransfers.Areas;

public record CreateAreaDto(
    string Name,
    AreaType Type,
    string? ParentId,
    double? Latitude,
    double? Longtitude
);