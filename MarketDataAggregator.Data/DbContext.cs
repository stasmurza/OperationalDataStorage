using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Interests;
using MarketDataAggregator.Entities.Ohlcs;
using MarketDataAggregator.Entities.Positions;
using MarketDataAggregator.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MarketDataAggregator.Data;

public class DbContext : IContext, IDisposable
{
    public IMongoCollection<Interest> Interests => Database.GetCollection<Interest>(CollectionNames.Interests);
    
    public IMongoCollection<Ohlc> Ohlcs => Database.GetCollection<Ohlc>(CollectionNames.Ohlcs);

    public IMongoCollection<Position> Positions => Database.GetCollection<Position>(CollectionNames.Positions);

    public IMongoDatabase Database { get; }

    public IMongoClient Client { get; }

    private bool disposed = false;

    public DbContext(IOptions<DatabaseSettings> options)
    {
        Client = new MongoClient(options.Value.ConnectionString);
        Database = Client.GetDatabase(options.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return Database.GetCollection<T>(name);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;

        // If disposing equals true, dispose all managed
        // and unmanaged resources.
        if (disposing)
        {
            // Dispose managed resources.
        }

        // Call the appropriate methods to clean up
        // unmanaged resources here.
        Client.Cluster.Dispose();

        // Note disposing has been done.
        disposed = true;
    }

    ~DbContext()
    {
        Dispose(false);
    }

}