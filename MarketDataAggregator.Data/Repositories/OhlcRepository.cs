using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Ohlcs;

namespace MarketDataAggregator.Data.Repositories;

public class OhlcRepository(IContext dbContext) : Repository<Ohlc>(dbContext.Ohlcs) {}
