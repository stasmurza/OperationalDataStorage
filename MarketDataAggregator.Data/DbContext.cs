using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Models.Entities.Events;
using MarketDataAggregator.Models.Entities.Ohlcs;
using MarketDataAggregator.Models.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MarketDataAggregator.Data;

public class DbContext : IContext, IDisposable
{
    public IMongoCollection<Event> Events => Database.GetCollection<Event>(CollectionNames.Events);
    
    public IMongoCollection<Ohlc> Ohlcs => Database.GetCollection<Ohlc>(CollectionNames.Ohlcs);
    
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