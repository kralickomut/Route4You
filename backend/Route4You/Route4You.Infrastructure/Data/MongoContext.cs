using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Route4You.Infrastructure.Data;

public sealed class MongoContext
{
    public IMongoDatabase Db { get; } 
    
    
    public MongoContext(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        this.Db = client.GetDatabase(options.Value.DatabaseName);
    }
    
    public IMongoCollection<T> GetCollection<T>(string name) => this.Db.GetCollection<T>(name);
}