namespace Route4You.Infrastructure;

public sealed class MongoOptions
{
    public string ConnectionString { get; init; } = default!;
    public string DatabaseName { get; init; } = default!;
}