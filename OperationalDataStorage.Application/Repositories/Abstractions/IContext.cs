using OperationalDataStorage.Domain.Entities.Ohlcs;
using OperationalDataStorage.Domain.Entities.Positions;
using MongoDB.Driver;

namespace OperationalDataStorage.Application.Repositories.Abstractions;

public interface IContext
{
    IMongoDatabase Database { get; }

    public IMongoClient Client { get; }

    IMongoCollection<Ohlc> Ohlcs { get; }
    
    IMongoCollection<Position> Positions { get; }

    public IMongoCollection<T> GetCollection<T>(string name);
}
