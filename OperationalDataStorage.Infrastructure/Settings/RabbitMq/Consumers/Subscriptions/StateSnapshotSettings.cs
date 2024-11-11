using OperationalDataStorage.Application.Settings;

namespace OperationalDataStorage.Infrastructure.Settings.RabbitMq.Consumers.Subscriptions;

public class StateSnapshotSettings : IValidatable
{
    public required string ExchangeName { get; set; }

    public required string QueueName { get; set; }

    public required IEnumerable<string> RoutingKeys { get; set; }

    public void Validate()
    {
        ArgumentNullException.ThrowIfNull(ExchangeName);
        ArgumentNullException.ThrowIfNull(QueueName);
        ArgumentNullException.ThrowIfNull(RoutingKeys);
        ArgumentNullException.ThrowIfNull(!RoutingKeys.Any());
    }
}
