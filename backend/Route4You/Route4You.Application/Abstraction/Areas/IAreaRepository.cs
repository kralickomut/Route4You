using Route4You.Domain.Areas;

namespace Route4You.Application.Abstraction.Areas;

public interface IAreaRepository
{
    Task<Area?> GetAsync(string id, CancellationToken ct = default);
    Task AddAsync(Area area, CancellationToken ct = default);
    Task<IReadOnlyList<Area>> GetByParentAsync(string? parentId, CancellationToken ct);
    Task IncrementChildrenCountAsync(string parentId, CancellationToken ct);
    
    Task IncrementRoutesCountAsync(string areaId, CancellationToken ct);
    Task IncrementRoutesCountManyAsync(IEnumerable<string> areaIds, CancellationToken ct);
    Task UpdateFieldsAsync(string id, string name, AreaType type, double? lat, double? lng, CancellationToken ct);
    
    Task<IReadOnlyList<string>> GetSubtreeAreaIdsAsync(string rootAreaId, CancellationToken ct);
    Task DeleteManyAsync(IEnumerable<string> areaIds, CancellationToken ct);
    
    Task DecrementRoutesCountManyAsync(IEnumerable<string> areaIds, CancellationToken ct);
    
    Task<IReadOnlyList<Area>> GetAllAsync(CancellationToken ct);
    Task SetCountsAsync(string areaId, int childrenCount, int routesCount, CancellationToken ct);

}