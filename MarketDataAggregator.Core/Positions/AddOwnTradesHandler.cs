using MarketDataAggregator.Core.Orders.Extensions;
using MarketDataAggregator.Core.Positions.Extensions;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Positions;
using MarketDataAggregator.Models.Positions;
using Microsoft.Extensions.Logging;

namespace MarketDataAggregator.Core.Positions;

public class AddOwnTradesHandler
{
    private readonly ILogger<AddOwnTradesHandler> logger;
    private readonly IContext context;
    private readonly IRepository<Position> positionRepository;

    public AddOwnTradesHandler(
        ILogger<AddOwnTradesHandler> logger,
        IContext context,
        IRepository<Position> positionRepository)
    {
        this.logger = logger;
        this.context = context;
        this.positionRepository = positionRepository;
    }

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
            entity = Create(dto);
            await positionRepository.CreateAsync(entity);
        }
        else
        {
            entity.Apply(dto);
            await positionRepository.UpdateAsync(entity);
        }
    }

    private static Position Create(OwnTradeDto dto) => new()
    {
        Symbol = dto.Symbol,
        Strategy = dto.Strategy,
        Quantity = dto.Quantity,
        EntryPrice = dto.Price,
        Direction = Enum.Parse<Entities.Direction>(dto.Direction.ToString()),
    };
}
