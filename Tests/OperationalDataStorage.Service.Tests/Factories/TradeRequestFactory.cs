using EventStore.Contracts;
using EventStore.Contracts.TradeRequests;

namespace OperationalDataStorage.Service.Tests.Factories;

public static class TradeRequestFactory
{
    public static TradeRequest GenerateLongTradeRequest(string symbol, string strategy)
    {
        return new TradeRequest()
        {
            EventId = Guid.NewGuid(),
            EventDateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            Symbol = symbol,
            Strategy = strategy,
            Direction = Direction.Buy,
            Quantity = 1,
            FinalPositionQuantity = 1,
            Price = 65000,
            ProcessBeforeUtc = DateTime.UtcNow.AddMinutes(5),
            StopOrder = true,
            StopPrice = 60000
        };
    }

    public static TradeRequest GenerateShortTradeRequest(string symbol, string strategy)
    {
        return new TradeRequest()
        {
            Id = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            Symbol = symbol,
            Strategy = strategy,
            Direction = Direction.Sell,
            Quantity = 1,
            FinalPositionQuantity = 1,
            Price = 65000,
            ProcessBeforeUtc = DateTime.UtcNow.AddMinutes(5),
            StopOrder = true,
            StopPrice = 60000
        };
    }
}
