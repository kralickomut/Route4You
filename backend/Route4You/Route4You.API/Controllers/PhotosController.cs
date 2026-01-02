using Microsoft.AspNetCore.Mvc;
using Route4You.Application.Abstraction.Images;

namespace Route4You.Controllers;

[ApiController]
[Route("api/photos")]
public sealed class PhotosController : ControllerBase
{
	private readonly IImageStorage _imageStorage;

	public PhotosController(IImageStorage imageStorage)
	{
		this._imageStorage = imageStorage;
	}

	// POST api/photos
	[HttpPost]
	[RequestSizeLimit(20_000_000)] // e.g. 20 MB
	public async Task<ActionResult<string>> Upload([FromForm] IFormFile file, CancellationToken ct)
	{
		if (file is null || file.Length == 0)
			return this.BadRequest("File is required.");

		await using var stream = file.OpenReadStream();

		var url = await this._imageStorage.UploadAsync(
			stream,
			file.ContentType,
			file.FileName,
			ct);

		return this.Ok(url);
	}
}