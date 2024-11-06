using MarketDataAggregator.Entities.Interests;
using MarketDataAggregator.Entities.Ohlcs;
using MarketDataAggregator.Entities.Positions;
using MarketDataAggregator.Models.Entities.Events;
using MarketDataAggregator.Models.Entities.Ohlcs;
using MarketDataAggregator.Models.Positions;
using MongoDB.Driver;

namespace MarketDataAggregator.Core.Repositories.Abstractions;

public interface IContext
{
    IMongoDatabase Database { get; }

    public IMongoClient Client { get; }

    IMongoCollection<Interest> Interests { get; }

    IMongoCollection<Ohlc> Ohlcs { get; }
    
    IMongoCollection<Position> Positions { get; }

    public IMongoCollection<T> GetCollection<T>(string name);
}
