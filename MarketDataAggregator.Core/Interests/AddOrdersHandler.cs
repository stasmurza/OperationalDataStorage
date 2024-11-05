using MarketDataAggregator.Core.Interests.Extensions;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Interests;
using MarketDataAggregator.Models.Interests;
using Microsoft.Extensions.Logging;

namespace MarketDataAggregator.Core.Interests;

public class AddOrdersHandler
{
    private readonly ILogger<AddOrdersHandler> logger;
    private readonly IRepository<Interest> interestRepository;

    public AddOrdersHandler(
        ILogger<AddOrdersHandler> logger,
        IRepository<Interest> interestRepository)
    {
        this.logger = logger;
        this.interestRepository = interestRepository;
    }

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
            entity = Create(dto);
            await interestRepository.CreateAsync(entity);
        }
        else
        {
            entity.Apply(dto);
            await interestRepository.UpdateAsync(entity);
        }
    }

    private static Interest Create(OrderDto dto) => new()
    {
        Symbol = dto.Symbol,
        Strategy = dto.Strategy,
        Quantity = dto.Quantity,
        AveragePrice = dto.Price,
        Direction = Enum.Parse<Entities.Direction>(dto.Direction.ToString()),
    };
}
