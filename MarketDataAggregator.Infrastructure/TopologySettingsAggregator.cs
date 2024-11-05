using MarketDataAggregator.Infrastructure.Settings.RabbitMq.Subscriptions;

namespace MarketDataAggregator.Infrastructure;

public class TopologySettingsAggregator
{
    public Topology Topology { get; } = new();

    public TopologySettingsAggregator(
        EventsSnapshotSettings marketDataSettings,
        OwnTradeSettings ownTradeSettings)
    {
        AddTopology(marketDataSettings.ExchangeName, marketDataSettings.QueueName, marketDataSettings.RoutingKeys);
        AddTopology(ownTradeSettings.ExchangeName, ownTradeSettings.QueueName, ownTradeSettings.RoutingKeys);
    }

    public IEnumerable<string> GetExchanges() => Topology.GetExchanhes();

    public IEnumerable<string> GetQueues(string exchange) => Topology.GetQueues(exchange);

    public IEnumerable<string> GetRoutingKeys(string exchange, string queue) => Topology.GetRoutingKeys(exchange, queue);

    private void AddTopology(string exchange, string queue, IEnumerable<string> routingKeys) =>
        Topology.AddTopology(exchange, queue, routingKeys);
}
