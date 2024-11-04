using EventStore.Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using EventStore.Models.Events;
using MarketDataAggregator.Infrastructure;

namespace EventStore.Infrastructure;

public class RabbitMqConsumer : IDisposable
{
    private readonly ILogger<RabbitMqConsumer> logger;
    private readonly IMediator mediator;
    private readonly RabbitMqClientSettings rabbitMQClientSettings;
    private readonly TopologySettingsAggregator topologySettingsAggregator;
    private readonly IConnection connection;
    private readonly IModel channel;
    private bool disposedValue;

    public RabbitMqConsumer(
        ILogger<RabbitMqConsumer> logger,
        IMediator mediator,
        IOptions<RabbitMqClientSettings> rabbitMQClientOptions,
        TopologySettingsAggregator topologySettingsAggregator)
    {
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions);
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions.Value);
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions.Value.HostName);
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions.Value.UserName);
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions.Value.Password);
        ArgumentNullException.ThrowIfNull(topologySettingsAggregator);
        
        this.logger = logger;
        this.mediator = mediator;

        rabbitMQClientSettings = rabbitMQClientOptions.Value;
        this.topologySettingsAggregator = topologySettingsAggregator;

        var factory = new ConnectionFactory()
        {
            HostName = rabbitMQClientSettings.HostName,
            UserName = rabbitMQClientSettings.UserName,
            Password = rabbitMQClientSettings.Password,
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            var entityId = GetHeader<string>(ea.BasicProperties.Headers, HeadersName.EntityIdName);
            this.logger.LogInformation($" [x] Received '{routingKey}':'{message}'");
            var input = new AddEventInput
            {
                EventType = EventTypeConvertor.GetEventType(routingKey).ToString(),
                EntityType = EntityTypeConvertor.GetEntityType(routingKey).ToString(),
                EntityId = entityId,
                EventData = message,
            };
            mediator.Send(input);
        };

        foreach (var exchangeName in topologySettingsAggregator.GetExchanges())
        {
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: false, autoDelete: false);

            foreach (var queueName in topologySettingsAggregator.GetQueues(exchangeName))
            {
                channel.QueueDeclare(queueName, durable: false, autoDelete: false);

                foreach (var bindingKey in this.topologySettingsAggregator.GetRoutingKeys(exchangeName, queueName))
                {
                    channel.QueueBind(
                        queue: queueName,
                        exchange: exchangeName,
                        routingKey: bindingKey);
                }

                channel.BasicConsume(
                    queue: queueName,
                    autoAck: true,
                    consumer: consumer);
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects)
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer
            // Set large fields to null
            channel.Close();
            connection.Close();
            channel.Dispose();
            connection.Dispose();
            disposedValue = true;
        }
    }

    // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~RabbitMqConsumer()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private T GetHeader<T>(IDictionary<string, object> headers, string key) where T : class
    {
        if (!headers.TryGetValue(key, out object? value) || value is null)
        {
            throw new InvalidOperationException($"Header {HeadersName.EntityIdName} is not found.");
        }

        var property = value as T ?? throw new InvalidOperationException($"Header {HeadersName.EntityIdName} is not found.");
        
        return property;
    }
}
