using MarketDataAggregator.Application.Repositories.Abstractions;
using MarketDataAggregator.Domain.Entities.Positions;

namespace MarketDataAggregator.Persistence.Repositories;

public class PositionRepository(IContext dbContext) : Repository<Position>(dbContext.Positions) {}
