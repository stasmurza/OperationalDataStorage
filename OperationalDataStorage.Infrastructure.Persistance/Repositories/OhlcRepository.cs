using OperationalDataStorage.Application.Repositories.Abstractions;
using OperationalDataStorage.Domain.Entities.Ohlcs;

namespace OperationalDataStorage.Infrastructure.Persistence.Repositories;

public class OhlcRepository(IContext dbContext) : Repository<Ohlc>(dbContext.Ohlcs) { }
