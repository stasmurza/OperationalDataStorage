using MarketDataAggregator.Models.Entities.Events;
using MarketDataAggregator.Models.Entities.Ohlcs;
using MarketDataAggregator.Models.Positions;
using MongoDB.Driver;

namespace MarketDataAggregator.Core.Repositories.Abstractions;

public interface IContext
{
    IMongoDatabase Database { get; }

    public IMongoClient Client { get; }

    IMongoCollection<Event> Events { get; }

    IMongoCollection<Ohlc> Ohlcs { get; }
    
    IMongoCollection<OwnTradeDto> OwnTrades { get; }

    public IMongoCollection<T> GetCollection<T>(string name);
}
