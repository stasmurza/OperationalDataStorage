using MarketDataAggregator.Application.Interests.Extensions;
using MarketDataAggregator.Application.Repositories.Abstractions;
using MarketDataAggregator.Application.Models.Interests;
using MediatR;
using MarketDataAggregator.Domain.Entities.Interests;

namespace MarketDataAggregator.Application.Interests;

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
