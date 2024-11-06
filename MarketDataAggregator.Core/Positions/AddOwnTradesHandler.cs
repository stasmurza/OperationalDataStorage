using MarketDataAggregator.Core.Positions.Extensions;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Positions;
using MarketDataAggregator.Models.Positions;
using MediatR;

namespace MarketDataAggregator.Core.Positions;

public class AddOwnTradesHandler(IRepository<Position> positionRepository) : IRequestHandler<AddOwnTradesInput>
{
    public async Task Handle(AddOwnTradesInput input, CancellationToken cancellationToken)
    {
        foreach (var dto in input.Dtos) await AddOwnTradeAsync(dto, cancellationToken);
    }

    public async Task AddOwnTradeAsync(OwnTradeDto dto, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;

        var entity = await positionRepository.FirstOrDefaultAsync(i => i.Symbol == dto.Symbol && i.Strategy == dto.Strategy);
        if (entity is null)
        {
            entity = dto.ToPositionEntity();
            await positionRepository.CreateAsync(entity);
        }
        else
        {
            entity.Apply(dto);
            if (entity.Quantity == 0) await positionRepository.DeleteAsync(entity.Id);
            else await positionRepository.UpdateAsync(entity);
        }
    }
}
