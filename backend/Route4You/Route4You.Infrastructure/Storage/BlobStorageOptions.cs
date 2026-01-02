namespace Route4You.Infrastructure.Storage;

public sealed class BlobStorageOptions
{
	public string ConnectionString { get; init; } = default!;
	public string ContainerName { get; init; } = default!;
}