using Route4You.Application.DataTransfers.Ascents;

namespace Route4You.Application.Abstraction.Ascents;

public interface IAscentService
{
	Task<AscentVm> RecordAsync(RecordAscentDto dto, CancellationToken ct);
	Task<IReadOnlyList<AscentVm>> GetByUserAsync(string userId, CancellationToken ct);
}