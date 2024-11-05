using MarketDataAggregator.Entities.Ohlcs;
using MarketDataAggregator.Entities.Positions;
using MarketDataAggregator.Models.OwnTrades;

namespace MarketDataAggregator.Core.Positions.Extensions;

public static class PositionExtensions
{
    public static void Apply(this Position position, OwnTradeDto dto)
    {
        if (position.Symbol != dto.Symbol) throw new ArgumentOutOfRangeException(nameof(dto));
        if (position.Strategy != dto.Strategy) throw new ArgumentOutOfRangeException(nameof(dto));

        if (position.Direction.ToString() == dto.Direction.ToString())
        {
            position.Quantity += dto.Quantity;
            position.EntryPrice 
        }
        else
        {

        }
        position.Quantity = dto.Open;
        position.Close = dto.Close;
        position.High = dto.High;
        position.Low = dto.Low;
        position.Volume = dto.Volume;
    }
}
