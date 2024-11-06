using MarketDataAggregator.Core.Interests.Extensions;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Interests;
using MarketDataAggregator.Models.Interests;
using MediatR;

namespace MarketDataAggregator.Core.Interests;

public class AddOrdersHandler(IRepository<Interest> interestRepository) : IRequestHandler<AddOrdersInput>
{
    public async Task Handle(AddOrdersInput input, CancellationToken cancellationToken)
    {
        foreach (var dto in input.Dtos) await AddOrderAsync(dto, cancellationToken);
    }

    public async Task AddOrderAsync(OrderDto dto, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;

        var entity = await interestRepository.FirstOrDefaultAsync(i => i.Symbol == dto.Symbol && i.Strategy == dto.Strategy);
        if (entity is null)
        {
            entity = dto.ToInterestEntity();
            await interestRepository.CreateAsync(entity);
        }
        else
        {
            entity.Apply(dto);
            if (entity.Quantity == 0) await interestRepository.DeleteAsync(entity.Id);
            else await interestRepository.UpdateAsync(entity);
        }
    }
}
