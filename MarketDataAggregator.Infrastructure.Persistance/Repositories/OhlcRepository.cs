using MarketDataAggregator.Application.Repositories.Abstractions;
using MarketDataAggregator.Domain.Entities.Ohlcs;

namespace MarketDataAggregator.Persistence.Repositories;

public class OhlcRepository(IContext dbContext) : Repository<Ohlc>(dbContext.Ohlcs) {}
