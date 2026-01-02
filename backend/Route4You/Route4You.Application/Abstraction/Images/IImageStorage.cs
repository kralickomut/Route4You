namespace Route4You.Application.Abstraction.Images;

public interface IImageStorage
{
	Task<string> UploadAsync(
		Stream content,
		string contentType,
		string fileName,
		CancellationToken ct);
}