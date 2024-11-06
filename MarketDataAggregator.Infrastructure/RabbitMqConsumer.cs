using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using MarketDataAggregator.Infrastructure.Settings.RabbitMq;
using MarketDataAggregator.Infrastructure.Settings.RabbitMq.Subscriptions;
using MarketDataAggregator.Contracts.Events;
using System.Text.Json;
using MarketDataAggregator.Models.Ohlcs;
using MarketDataAggregator.Models.Interests;
using MarketDataAggregator.Models.Positions;

namespace EventStore.Infrastructure;

public class RabbitMqConsumer : IDisposable
{
    private readonly ILogger<RabbitMqConsumer> logger;
    private readonly IMediator mediator;
    private readonly RabbitMqClientSettings rabbitMQClientSettings;
    private readonly EventsSnapshotSettings eventsSnapshotSettings;
    private readonly IConnection connection;
    private readonly IModel channel;
    private bool disposedValue;

    public RabbitMqConsumer(
        ILogger<RabbitMqConsumer> logger,
        IMediator mediator,
        IOptions<RabbitMqClientSettings> rabbitMQClientOptions,
        EventsSnapshotSettings eventsSnapshotSettings)
    {
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions);
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions.Value);
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions.Value.HostName);
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions.Value.UserName);
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions.Value.Password);
        ArgumentNullException.ThrowIfNull(eventsSnapshotSettings);
        
        this.logger = logger;
        this.mediator = mediator;
        this.eventsSnapshotSettings = eventsSnapshotSettings;
        rabbitMQClientSettings = rabbitMQClientOptions.Value;

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

            var eventsSnapshort = JsonSerializer.Deserialize<EventsSnapshot>(message);
            if (eventsSnapshort is null) throw new NullReferenceException(nameof(eventsSnapshort));
            if (!eventsSnapshort.Events.Any()) return;
            var dtosByEventType = eventsSnapshort.Events.GroupBy(e => e.EventType);
            foreach(var group in dtosByEventType)
            {
                ProcessEvents(group.Key, group.Select(i => i.EventData));
            }
            
        };

        channel.ExchangeDeclare(eventsSnapshotSettings.ExchangeName, ExchangeType.Direct, durable: false, autoDelete: false);
        channel.QueueDeclare(eventsSnapshotSettings.QueueName, durable: false, autoDelete: false);
        foreach (var bindingKey in this.eventsSnapshotSettings.RoutingKeys)
        {
            channel.QueueBind(
                queue: eventsSnapshotSettings.QueueName,
                exchange: eventsSnapshotSettings.ExchangeName,
                routingKey: bindingKey);
        }

        channel.BasicConsume(
            queue: eventsSnapshotSettings.QueueName,
            autoAck: true,
            consumer: consumer);
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

    private void ProcessEvents(EventType eventType, IEnumerable<string> events)
    {
        switch (eventType)
        {
            case EventType.MarketOrderReceived:
                {
                    var dtos = events.Select(Deserialize<MarketDataAggregator.Models.Interests.OrderDto>);
                    var input = new AddOrdersInput { Dtos = dtos };
                    mediator.Send(input);
                }
                break;

            case EventType.OhlcReceived:
                {
                    var dtos = events.Select(Deserialize<MarketDataAggregator.Models.Ohlcs.OhlcDto>);
                    var input = new AddOhlcsInput { Dtos = dtos };
                    mediator.Send(input);
                }
                break;

            case EventType.OwnTradeReceived:
                {
                    var dtos = events.Select(Deserialize<MarketDataAggregator.Models.Positions.OwnTradeDto>);
                    var input = new AddOwnTradesInput { Dtos = dtos };
                    mediator.Send(input);
                }
                break;

            default:
                break;


        }
    }

    private T Deserialize<T>(string eventData)
    {
        var dto = JsonSerializer.Deserialize<T>(eventData);
        return dto ?? throw new NullReferenceException(nameof(dto));
    }
}
