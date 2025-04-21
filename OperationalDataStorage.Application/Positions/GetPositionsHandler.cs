using OperationalDataStorage.Application.Positions.Extensions;
using OperationalDataStorage.Application.Repositories.Abstractions;
using OperationalDataStorage.Application.Models.Positions;
using MediatR;
using OperationalDataStorage.Domain.Entities.Positions;

namespace OperationalDataStorage.Application.Positions;

public class GetPositionsHandler(IRepository<Position> positionRepository) : IRequestHandler<GetPositionsInput, GetPositionsOutput>
{
    private readonly IRepository<Position> positionRepository = positionRepository;

    public async Task<GetPositionsOutput> Handle(GetPositionsInput input, CancellationToken cancellationToken)
    {
        var positions = await positionRepository.GetAsync(i => i.Strategy == input.Strategy);

        return new GetPositionsOutput()
        {
            Positions = positions.Select(i => i.ToDto()),
        };
    }
}
