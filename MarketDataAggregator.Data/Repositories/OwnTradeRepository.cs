using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Models.OwnTrades;

namespace MarketDataAggregator.Data.Repositories;

public class OwnTradeRepository(IContext dbContext) : Repository<OwnTradeDto>(dbContext.OwnTrades) {}
