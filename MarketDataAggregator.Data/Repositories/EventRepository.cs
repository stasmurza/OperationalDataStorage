using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Models.Entities.Events;

namespace MarketDataAggregator.Data.Repositories;

public class EventRepository(IContext dbContext) : Repository<Event>(dbContext.Events) {}
