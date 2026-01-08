using Microsoft.AspNetCore.Mvc;
using Route4You.Application.Abstraction.Routes;
using Route4You.Application.DataTransfers.Routes;

namespace Route4You.Controllers;

[ApiController]
[Route("api/routes")]
public sealed class RoutesController : ControllerBase
{
	private readonly IRouteService _service;

	public RoutesController(IRouteService service)
	{
		this._service = service;
	}

	[HttpPost]
	public async Task<ActionResult<RouteVm>> Create([FromBody] CreateRouteDto dto, CancellationToken ct)
	{
		var vm = await this._service.CreateAsync(dto, ct);
		return this.CreatedAtAction(nameof(this.GetById), new { id = vm.Id }, vm);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<RouteVm>> GetById(string id, CancellationToken ct)
	{
		var vm = await this._service.GetAsync(id, ct);
		return vm is null ? this.NotFound() : this.Ok(vm);
	}

	[HttpGet("by-area/{areaId}")]
	public async Task<ActionResult<IReadOnlyList<RouteVm>>> GetByArea(string areaId, CancellationToken ct)
	{
		var list = await this._service.GetByAreaAsync(areaId, ct);
		return this.Ok(list);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult<RouteVm>> Update(string id, [FromBody] UpdateRouteDto dto, CancellationToken ct)
	{
		if (dto.Id != id) return this.BadRequest("Id mismatch.");

		var vm = await this._service.UpdateAsync(dto, ct);
		return this.Ok(vm);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(string id, CancellationToken ct)
	{
		await this._service.DeleteAsync(id, ct);
		return this.NoContent();
	}
}