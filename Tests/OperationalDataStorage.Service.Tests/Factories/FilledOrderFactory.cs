using OperationalDataStorage.Contracts.Positions;

namespace OperationalDataStorage.Service.Tests.Factories;

public static class FilledOrderFactory
{
    public static FilledOrder GenerateFilledOrder(string symbol, string strategy, Contracts.Direction direction, decimal volume, decimal price)
    {
        return new FilledOrder()
        {
            OrderId = Guid.NewGuid(),
            Symbol = symbol,
            Strategy = strategy,
            Direction = direction,
            FilledQuantity = volume,
            FinalPositionQuantity = volume,
            AverageFillPrice = price,
        };
    }
}
