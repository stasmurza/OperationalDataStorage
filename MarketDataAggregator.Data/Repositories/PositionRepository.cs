using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Positions;

namespace MarketDataAggregator.Data.Repositories;

public class PositionRepository(IContext dbContext) : Repository<Position>(dbContext.Positions) {}
