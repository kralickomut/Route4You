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
}