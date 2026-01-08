using Microsoft.AspNetCore.Mvc;
using Route4You.Application.Abstraction.Users;
using Route4You.Application.DataTransfers.Users;

namespace Route4You.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
	private readonly IUserService _service;

	public UsersController(IUserService service)
	{
		this._service = service;
	}

	[HttpGet]
	public async Task<ActionResult<IReadOnlyList<UserVm>>> GetAll(CancellationToken ct)
	{
		return this.Ok(await this._service.GetAllAsync(ct));
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<UserVm>> GetById(string id, CancellationToken ct)
	{
		var vm = await this._service.GetAsync(id, ct);
		return vm is null ? this.NotFound() : this.Ok(vm);
	}
}