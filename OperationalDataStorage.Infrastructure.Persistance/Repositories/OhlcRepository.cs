using OperationalDataStorage.Application.Repositories.Abstractions;
using OperationalDataStorage.Domain.Entities.Ohlcs;

namespace OperationalDataStorage.Persistence.Repositories;

public class OhlcRepository(IContext dbContext) : Repository<Ohlc>(dbContext.Ohlcs) {}
