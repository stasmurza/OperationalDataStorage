using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Options;
using System.Text.Json;
using MarketDataAggregator.Infrastructure.Settings.RabbitMq;
using MarketDataAggregator.Infrastructure.Settings.RabbitMq.Consumers.Subscriptions;
using MarketDataAggregator.Contracts.StateSnapshots;

namespace MarketDataAggregator.Infrastructure;

public class StateSnapshotPublisher : IDisposable
{
    private readonly RabbitMqClientSettings rabbitMQClientSettings;
    private readonly StateSnapshotSettings stateSnapshotSettings;
    private readonly IConnection connection;
    private readonly IModel channel;
    private bool disposed = false;

    public StateSnapshotPublisher(
        IOptions<RabbitMqClientSettings> rabbitMQClientOptions,
        IOptions<StateSnapshotSettings> stateSnapshotOptions)
    {
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions);
        ArgumentNullException.ThrowIfNull(stateSnapshotOptions);
        rabbitMQClientOptions.Value.Validate();
        stateSnapshotOptions.Value.Validate();

        rabbitMQClientSettings = rabbitMQClientOptions.Value;
        this.stateSnapshotSettings = stateSnapshotOptions.Value;

        var factory = new ConnectionFactory()
        {
            HostName = rabbitMQClientSettings.HostName,
            UserName = rabbitMQClientSettings.UserName,
            Password = rabbitMQClientSettings.Password,
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare(stateSnapshotSettings.ExchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
    }

    public void PublishEventSnapshot(StateSnapshotResponse stateSnapshot, CancellationToken cancellationToken)
    {
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(stateSnapshot));
        foreach (var routingKey in stateSnapshotSettings.RoutingKeys)
        {
            if (cancellationToken.IsCancellationRequested) return;

            channel.BasicPublish(
                exchange: stateSnapshotSettings.ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing)
        {
            //Clean manageable resources.
        };

        //Clean unmanageable resources.
        channel.Close();
        connection.Close();
        channel.Dispose();
        connection.Dispose();

        disposed = true;
    }

    ~StateSnapshotPublisher()
    {
        Dispose(false);
    }
}
