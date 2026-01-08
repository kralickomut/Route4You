using Microsoft.AspNetCore.Mvc;
using Route4You.Application.Abstraction.Areas;
using Route4You.Application.DataTransfers.Areas;

namespace Route4You.Controllers;

[ApiController]
[Route("api/areas")]
public sealed class AreasController : ControllerBase
{
	private readonly IAreaService _service;

	public AreasController(IAreaService service)
	{
		this._service = service;
	}

	[HttpPost]
	public async Task<ActionResult<AreaVm>> Create([FromBody] CreateAreaDto dto, CancellationToken ct)
	{
		var vm = await this._service.CreateAsync(dto, ct);
		return this.CreatedAtAction(nameof(this.GetById), new { id = vm.Id }, vm);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<AreaVm>> GetById(string id, CancellationToken ct)
	{
		var vm = await this._service.GetAsync(id, ct);
		return vm is null ? this.NotFound() : this.Ok(vm);
	}

	[HttpGet]
	public async Task<ActionResult<IReadOnlyList<AreaVm>>> GetChildren(
		[FromQuery] string? parentId,
		CancellationToken ct)
	{
		var list = await this._service.GetChildrenAsync(parentId, ct);
		return this.Ok(list);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult<AreaVm>> Update(string id, [FromBody] UpdateAreaDto dto, CancellationToken ct)
	{
		if (dto.Id != id) return this.BadRequest("Id mismatch.");

		var vm = await this._service.UpdateAsync(dto, ct);
		return this.Ok(vm);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(string id, CancellationToken ct)
	{
		await this._service.DeleteRecursiveAsync(id, ct); // delete
		await this._service.RebuildCountsAsync(ct); // bruteforce sync
		return this.NoContent();
	}
}