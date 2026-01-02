using Microsoft.AspNetCore.Mvc;
using Route4You.Application.Abstraction.Ascents;
using Route4You.Application.DataTransfers.Ascents;

namespace Route4You.Controllers;

[ApiController]
[Route("api/ascents")]
public class AscentController : ControllerBase
{
	private readonly IAscentService _ascentService;
	
	public AscentController(IAscentService ascentService)
	{
		this._ascentService = ascentService;
	}
	
	[HttpPost]
	public async Task<IActionResult> CreateOrSetRecord([FromBody] RecordAscentDto dto, CancellationToken ct)
	{
		var vm = await this._ascentService.RecordAsync(dto, ct);
		
		return this.Ok(vm);
	}
	
	// by user
	[HttpGet("by-user/{userId}")]
	public async Task<IActionResult> GetByUser(string userId, CancellationToken ct)
	{
		var list = await this._ascentService.GetByUserAsync(userId, ct);
		
		return this.Ok(list);
	}
}