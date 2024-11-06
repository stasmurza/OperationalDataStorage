using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Interests;

namespace MarketDataAggregator.Data.Repositories;

public class InterestRepository(IContext dbContext) : Repository<Interest>(dbContext.Interests) {}
