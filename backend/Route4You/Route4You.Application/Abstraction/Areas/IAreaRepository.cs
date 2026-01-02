using Route4You.Domain.Areas;

namespace Route4You.Application.Abstraction.Areas;

public interface IAreaRepository
{
    Task<Area?> GetAsync(string id, CancellationToken ct = default);
    Task AddAsync(Area area, CancellationToken ct = default);
    Task<IReadOnlyList<Area>> GetByParentAsync(string? parentId, CancellationToken ct);

}