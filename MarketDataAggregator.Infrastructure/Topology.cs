namespace MarketDataAggregator.Infrastructure;

public class Topology
{
    public HashSet<string> Exchanges { get; } = [];

    public Dictionary<string, HashSet<string>> Queues { get; } = [];

    public Dictionary<string, HashSet<string>> RoutingKeys { get; } = [];

    public IEnumerable<string> GetExchanhes() => Exchanges;

    public IEnumerable<string> GetQueues(string exchange) => Queues[exchange];

    public IEnumerable<string> GetRoutingKeys(string exchange, string queue) => Queues[exchange + queue];

    public void AddTopology(string exchange, string queue, IEnumerable<string> routingKeys)
    {
        Exchanges.Add(exchange);

        if (!Queues.TryGetValue(exchange, out var queues))
        {
            queues = [];
            Queues.Add(exchange, queues);
        }
        queues.Add(queue);

        var key = exchange + queue;
        if (!RoutingKeys.TryGetValue(key, out var routingKeysSet))
        {
            routingKeysSet = [];
            RoutingKeys.Add(exchange, routingKeysSet);
        }

        foreach (var routingKey in routingKeys)
        {
            routingKeysSet.Add(routingKey);
        }
    }
}
