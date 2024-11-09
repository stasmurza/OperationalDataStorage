using MarketDataAggregator.Application.Repositories.Abstractions;
using MarketDataAggregator.Domain.Entities.Interests;
using MarketDataAggregator.Persistence.Repositories;

namespace MarketDataAggregator.Infrastructure.Persistence.Repositories;

public class InterestRepository(IContext dbContext) : Repository<Interest>(dbContext.Interests) { }
