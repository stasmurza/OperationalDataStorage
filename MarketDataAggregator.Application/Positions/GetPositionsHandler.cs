using MarketDataAggregator.Application.Positions.Extensions;
using MarketDataAggregator.Application.Repositories.Abstractions;
using MarketDataAggregator.Application.Models.Positions;
using MediatR;
using MarketDataAggregator.Domain.Entities.Positions;

namespace MarketDataAggregator.Application.Positions;

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
