using MarketDataAggregator.Core.Positions.Extensions;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Positions;
using MarketDataAggregator.Models.Positions;
using MediatR;

namespace MarketDataAggregator.Core.Positions;

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
