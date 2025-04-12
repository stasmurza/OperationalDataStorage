using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using OperationalDataStorage.Contracts.Events;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using MongoDB.Driver;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq.Consumers.Subscriptions;

namespace OperationalDataStorage.Infrastructure;

public class EventsSnapshotConsumer : IDisposable
{
    private readonly ILogger<EventsSnapshotConsumer> logger;
    private readonly IMediator mediator;
    private readonly IMapper mapper;
    private readonly RabbitMqClientSettings rabbitMQClientSettings;
    private readonly EventsSnapshotSettings eventsSnapshotSettings;
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private bool disposedValue;

    public EventsSnapshotConsumer(
        ILogger<EventsSnapshotConsumer> logger,
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

        jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += Consumer_Received;
        channel.BasicConsume(
            queue: eventsSnapshotSettings.QueueName,
            autoAck: true,
            consumer: consumer);
    }

    private void Consumer_Received(object? sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = eventArgs.RoutingKey;
            var eventsSnapshort = JsonSerializer.Deserialize<EventsSnapshot>(message, jsonSerializerOptions);
            if (eventsSnapshort is null) throw new NullReferenceException(nameof(eventsSnapshort));
            if (!eventsSnapshort.NewEvents.Any()) return;
            LogEvents(eventsSnapshort.NewEvents.Select(i => JsonSerializer.Serialize(i, jsonSerializerOptions)));
            var dtosByEventType = eventsSnapshort.NewEvents.GroupBy(e => e.EventType);
            foreach (var group in dtosByEventType)
            {
                ProcessEvents(group.Key, group.Select(i => i.EventData));
            }
        }
        catch (Exception exception)
        {
            logger.LogError("{exceptionMessage}", exception.Message);
            throw;
        }
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

    private void ProcessEvents(EventType eventType, IEnumerable<string> events)
    {
        switch (eventType)
        {
            case EventType.OhlcReceived:
                {
                    var dtos = events.Select(Deserialize<Application.Models.Ohlcs.OhlcDto>);
                    var input = new Application.Models.Ohlcs.AddOhlcsInput { Dtos = dtos };
                    mediator.Send(input);
                }
                break;

            case EventType.TradeRequestReceived:
                {
                    var dtos = events.Select(Deserialize<Application.Models.Positions.OwnTradeDto>);
                    var input = new Application.Models.Positions.AddOwnTradesInput { Dtos = dtos };
                    mediator.Send(input);
                }
                break;

            case EventType.MarketOrderReceived:
                {
                    var dtos = events.Select(Deserialize<Contracts.Interests.OrderDto>);
                    var input = new Application.Models.Interests.AddOrdersInput
                    {
                        Dtos = dtos.Select(mapper.Map<Application.Models.Interests.OrderDto>)
                    };
                    mediator.Send(input);
                }
                break;

            case EventType.FilledOrderReceived:
                {
                    var dtos = events.Select(Deserialize<Application.Models.Positions.OwnTradeDto>);
                    var input = new Application.Models.Positions.AddOwnTradesInput { Dtos = dtos };
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
    ~EventsSnapshotConsumer()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }
}
