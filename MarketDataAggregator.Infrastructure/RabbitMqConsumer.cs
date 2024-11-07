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
using System.Text.Json.Serialization;
using AutoMapper;

namespace MarketDataAggregator.Infrastructure;

public class RabbitMqConsumer : IDisposable
{
    private readonly ILogger<RabbitMqConsumer> logger;
    private readonly IMediator mediator;
    private readonly IMapper mapper;
    private readonly RabbitMqClientSettings rabbitMQClientSettings;
    private readonly EventsSnapshotSettings eventsSnapshotSettings;
    private readonly IConnection connection;
    private readonly IModel channel;
    private bool disposedValue;

    public RabbitMqConsumer(
        ILogger<RabbitMqConsumer> logger,
        IMediator mediator,
        IOptions<RabbitMqClientSettings> rabbitMQClientOptions,
        IOptions<EventsSnapshotSettings> eventsSnapshotOptions,
        IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions);
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions.Value);
        ArgumentNullException.ThrowIfNull(eventsSnapshotOptions);
        ArgumentNullException.ThrowIfNull(eventsSnapshotOptions.Value);
        rabbitMQClientOptions.Value.Validate();
        eventsSnapshotOptions.Value.Validate();

        this.logger = logger;
        this.mediator = mediator;
        this.rabbitMQClientSettings = rabbitMQClientOptions.Value;
        this.eventsSnapshotSettings = eventsSnapshotOptions.Value;
        this.mapper = mapper;

        var factory = new ConnectionFactory()
        {
            HostName = rabbitMQClientSettings.HostName,
            UserName = rabbitMQClientSettings.UserName,
            Password = rabbitMQClientSettings.Password,
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare(eventsSnapshotSettings.ExchangeName, ExchangeType.Direct, durable: false, autoDelete: false);
        channel.QueueDeclare(eventsSnapshotSettings.QueueName, durable: false, autoDelete: false);
        foreach (var bindingKey in this.eventsSnapshotSettings.RoutingKeys)
        {
            channel.QueueBind(
                queue: eventsSnapshotSettings.QueueName,
                exchange: eventsSnapshotSettings.ExchangeName,
                routingKey: bindingKey);
        }

        var consumer = CreateConsumer();
        channel.BasicConsume(
            queue: eventsSnapshotSettings.QueueName,
            autoAck: true,
            consumer: consumer);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
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

    private EventingBasicConsumer CreateConsumer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            var eventsSnapshort = JsonSerializer.Deserialize<EventsSnapshot>(message, options);
            if (eventsSnapshort is null) throw new NullReferenceException(nameof(eventsSnapshort));
            if (!eventsSnapshort.Events.Any()) return;
            LogEvents(eventsSnapshort.Events.Select(i => JsonSerializer.Serialize(i, options)));
            var dtosByEventType = eventsSnapshort.Events.GroupBy(e => e.EventType);
            foreach (var group in dtosByEventType)
            {
                ProcessEvents(group.Key, group.Select(i => i.EventData));
            }

        };

        return consumer;
    }

    private void ProcessEvents(EventType eventType, IEnumerable<string> events)
    {
        switch (eventType)
        {
            case EventType.MarketOrderReceived:
                {
                    var dtos = events.Select(Deserialize<Contracts.Interests.OrderDto>);
                    var input = new Models.Interests.AddOrdersInput
                    {
                        Dtos = dtos.Select(mapper.Map<Models.Interests.OrderDto>)
                    };
                    mediator.Send(input);
                }
                break;

            case EventType.OhlcReceived:
                {
                    var dtos = events.Select(Deserialize<Models.Ohlcs.OhlcDto>);
                    var input = new Models.Ohlcs.AddOhlcsInput { Dtos = dtos };
                    mediator.Send(input);
                }
                break;

            case EventType.OwnTradeReceived:
                {
                    var dtos = events.Select(Deserialize<Models.Positions.OwnTradeDto>);
                    var input = new Models.Positions.AddOwnTradesInput { Dtos = dtos };
                    mediator.Send(input);
                }
                break;

            default:
                break;


        }
    }

    private static T Deserialize<T>(string eventData)
    {
        var dto = JsonSerializer.Deserialize<T>(eventData);
        return dto ?? throw new NullReferenceException(nameof(dto));
    }

    private void LogEvents(IEnumerable<string> events)
    {
        foreach(var @event in events)
        {
            logger.LogInformation("{event} received", @event);
        }
        
    }

    // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~RabbitMqConsumer()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }
}
