using OperationalDataStorage.Application.Repositories.Abstractions;
using OperationalDataStorage.Domain.Entities.Positions;

namespace OperationalDataStorage.Infrastructure.Persistence.Repositories;

public class PositionRepository(IContext dbContext) : Repository<Position>(dbContext.Positions) { }
