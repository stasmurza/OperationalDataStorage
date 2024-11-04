namespace MarketDataAggregator.Infrastructure.Settings.RabbitMq.Subscriptions;

public class OwnTradeSettings
{
    public required string ExchangeName { get; set; }

    public required string QueueName { get; set; }

    public required IEnumerable<string> RoutingKeys { get; set; }
}
