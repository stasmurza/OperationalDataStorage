using MarketDataAggregator.Entities.Ohlcs;
using MarketDataAggregator.Entities.Positions;
using MarketDataAggregator.Models.Positions;

namespace MarketDataAggregator.Core.Positions.Extensions;

public static class PositionExtensions
{
    public static void Apply(this Position position, OwnTradeDto dto)
    {
        if (position.Symbol != dto.Symbol) throw new ArgumentOutOfRangeException(nameof(dto));
        if (position.Strategy != dto.Strategy) throw new ArgumentOutOfRangeException(nameof(dto));

        var positionAmount = position.EntryPrice * position.Quantity;
        var tradeAmount = dto.Price * dto.Quantity;
        if (position.Direction.ToString() == dto.Direction.ToString())
        {
            position.Quantity += dto.Quantity;
            position.EntryPrice = (tradeAmount + positionAmount) / position.Quantity;
        }
        else
        {
            var quantity = Math.Max(dto.Quantity, position.Quantity) - Math.Min(dto.Quantity, position.Quantity);
            var entryPrice = quantity == 0 ? 0 : (Math.Max(positionAmount, tradeAmount) - Math.Min(positionAmount, tradeAmount)) / quantity;
            position.Quantity = quantity;
            position.EntryPrice = entryPrice;
            position.Direction = positionAmount >= tradeAmount ? position.Direction : Enum.Parse<Entities.Direction>(dto.Direction.ToString());
        }
    }

    public static PositionDto ToDto(this Position position) => new()
    {
        Id = position.Id,
        Symbol = position.Symbol,
        Strategy = position.Strategy,
        Quantity = position.Quantity,
        EntryPrice = position.EntryPrice,
        Direction = Enum.Parse<Models.Direction>(position.Direction.ToString()),
    };
}
