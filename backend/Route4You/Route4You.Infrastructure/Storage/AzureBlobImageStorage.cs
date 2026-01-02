using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Route4You.Application.Abstraction.Images;

namespace Route4You.Infrastructure.Storage;

public sealed class AzureBlobImageStorage : IImageStorage
{
	private readonly BlobContainerClient _containerClient;

	public AzureBlobImageStorage(IOptions<BlobStorageOptions> options)
	{
		var opts = options.Value;
		var serviceClient = new BlobServiceClient(opts.ConnectionString);

		this._containerClient = serviceClient.GetBlobContainerClient(opts.ContainerName);

		this._containerClient.CreateIfNotExists(PublicAccessType.Blob);
	}

	public async Task<string> UploadAsync(
		Stream content,
		string contentType,
		string fileName,
		CancellationToken ct)
	{
		var ext = Path.GetExtension(fileName);
		var blobName = $"{Guid.NewGuid():N}{ext}";

		var blobClient = this._containerClient.GetBlobClient(blobName);

		var headers = new BlobHttpHeaders
		{
			ContentType = contentType
		};

		await blobClient.UploadAsync(content, new BlobUploadOptions
		{
			HttpHeaders = headers
		}, ct);

		// You can store either blobName or full Uri; I’d store blobName in DB
		return blobClient.Uri.ToString();
	}
}