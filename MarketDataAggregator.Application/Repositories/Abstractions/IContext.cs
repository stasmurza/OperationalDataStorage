using MarketDataAggregator.Domain.Entities.Interests;
using MarketDataAggregator.Domain.Entities.Ohlcs;
using MarketDataAggregator.Domain.Entities.Positions;
using MongoDB.Driver;

namespace MarketDataAggregator.Application.Repositories.Abstractions;

public interface IContext
{
    IMongoDatabase Database { get; }

    public IMongoClient Client { get; }

    IMongoCollection<Interest> Interests { get; }

    IMongoCollection<Ohlc> Ohlcs { get; }
    
    IMongoCollection<Position> Positions { get; }

    public IMongoCollection<T> GetCollection<T>(string name);
}
