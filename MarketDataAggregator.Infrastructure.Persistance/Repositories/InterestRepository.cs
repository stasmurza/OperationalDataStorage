using MarketDataAggregator.Application.Repositories.Abstractions;
using MarketDataAggregator.Domain.Entities.Interests;

namespace MarketDataAggregator.Persistence.Repositories;

public class InterestRepository(IContext dbContext) : Repository<Interest>(dbContext.Interests) {}
