using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Route4You.Infrastructure.Data;

public sealed class Initializer(
    MongoContext context,
    ILogger<Initializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Quick connection test — run a simple command
            await context.Db.RunCommandAsync(
                (Command<BsonDocument>)"{ ping: 1 }",
                cancellationToken: cancellationToken
            );

            logger.LogInformation("✅ Successfully connected to MongoDB database '{DatabaseName}'.",
                context.Db.DatabaseNamespace.DatabaseName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Failed to connect to MongoDB.");
            throw; // fail fast if Mongo is not reachable
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}