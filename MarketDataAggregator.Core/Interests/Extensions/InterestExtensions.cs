using MarketDataAggregator.Entities.Interests;
using MarketDataAggregator.Models.Interests;

namespace MarketDataAggregator.Core.Interests.Extensions;

public static class InterestExtensions
{
    public static void Apply(this Interest interest, OrderDto dto)
    {
        if (interest.Symbol != dto.Symbol) throw new ArgumentOutOfRangeException(nameof(dto));
        if (interest.Strategy != dto.Strategy) throw new ArgumentOutOfRangeException(nameof(dto));

        var positionAmount = interest.AveragePrice * interest.Quantity;
        var tradeAmount = dto.Price * dto.Quantity;
        if (interest.Direction.ToString() == dto.Direction.ToString())
        {
            interest.Quantity += dto.Quantity;
            interest.AveragePrice = (tradeAmount + positionAmount) / interest.Quantity;
        }
        else
        {
            var quantity = Math.Max(dto.Quantity, interest.Quantity) - Math.Min(dto.Quantity, interest.Quantity);
            var entryPrice = quantity == 0 ? 0 : (Math.Max(positionAmount, tradeAmount) - Math.Min(positionAmount, tradeAmount)) / quantity;
            interest.Quantity = quantity;
            interest.AveragePrice = entryPrice;
            interest.Direction = positionAmount >= tradeAmount ? interest.Direction : Enum.Parse<Entities.Direction>(dto.Direction.ToString());
        }
    }

    public static InterestDto ToDto(this Interest interest) => new()
    {
        Id = interest.Id,
        Symbol = interest.Symbol,
        Strategy = interest.Strategy,
        Quantity = interest.Quantity,
        AveragePrice = interest.AveragePrice,
        Direction = Enum.Parse<Models.Direction>(interest.Direction.ToString()),
    };
}
