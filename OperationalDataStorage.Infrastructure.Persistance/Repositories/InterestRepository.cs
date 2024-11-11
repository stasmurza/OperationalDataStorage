using OperationalDataStorage.Application.Repositories.Abstractions;
using OperationalDataStorage.Domain.Entities.Interests;
using OperationalDataStorage.Persistence.Repositories;

namespace OperationalDataStorage.Infrastructure.Persistence.Repositories;

public class InterestRepository(IContext dbContext) : Repository<Interest>(dbContext.Interests) { }
