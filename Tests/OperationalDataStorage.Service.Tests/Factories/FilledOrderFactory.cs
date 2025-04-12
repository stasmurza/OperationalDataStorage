using EventStore.Contracts.Orders;
using EventStore.Contracts.TradeRequests;

namespace OperationalDataStorage.Service.Tests.Factories;

public static class FilledOrderFactory
{
    public static FilledOrder GenerateFilledOrder(TradeRequest tradeRequest)
    {
        return new FilledOrder()
        {
            EventId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            EventDateTime = DateTime.UtcNow,
            OrderId = Guid.NewGuid(),
            Symbol = tradeRequest.Symbol,
            Strategy = tradeRequest.Strategy,
            Direction = tradeRequest.Direction,
            FilledQuantity = tradeRequest.Quantity,
            FinalPositionQuantity = tradeRequest.FinalPositionQuantity,
            AverageFillPrice = tradeRequest.Price,
        };
    }
}
