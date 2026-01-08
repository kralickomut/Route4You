using Route4You.Application.DataTransfers;
using Route4You.Application.DataTransfers.Areas;

namespace Route4You.Application.Abstraction.Areas;

public interface IAreaService
{
    Task<AreaVm> CreateAsync(CreateAreaDto dto, CancellationToken ct);
    Task<AreaVm?> GetAsync(string id, CancellationToken ct);
    Task<IReadOnlyList<AreaVm>> GetChildrenAsync(string? parentId, CancellationToken ct);
    Task<AreaVm> UpdateAsync(UpdateAreaDto dto, CancellationToken ct);
    Task DeleteRecursiveAsync(string id, CancellationToken ct);
    
    Task RebuildCountsAsync(CancellationToken ct);
}