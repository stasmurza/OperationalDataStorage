using OperationalDataStorage.Application.Repositories.Abstractions;
using OperationalDataStorage.Domain.Entities.Interests;
using OperationalDataStorage.Domain.Entities.Ohlcs;
using OperationalDataStorage.Domain.Entities.Positions;
using OperationalDataStorage.Infrastructure.Settings;
using OperationalDataStorage.Persistence;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace OperationalDataStorage.Infrastructure.Persistence;

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
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Value);
        options.Value.Validate();

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