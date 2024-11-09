using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using MarketDataAggregator.Infrastructure.Settings.RabbitMq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using MarketDataAggregator.Infrastructure.Settings.RabbitMq.Consumers.Subscriptions;
using MarketDataAggregator.Contracts.StateSnapshots;
using MongoDB.Driver;

namespace MarketDataAggregator.Infrastructure;

public class StateSnapshotConsumer : IDisposable
{
    private readonly ILogger<EventsSnapshotConsumer> logger;
    private readonly IMediator mediator;
    private readonly IMapper mapper;
    private readonly StateSnapshotPublisher stateSnapshotPublisher;
    private readonly RabbitMqClientSettings rabbitMQClientSettings;
    private readonly StateSnapshotSettings stateSnapshotSettings;
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private bool disposedValue;

    public StateSnapshotConsumer(
        ILogger<EventsSnapshotConsumer> logger,
        IMediator mediator,
        IOptions<RabbitMqClientSettings> rabbitMQClientOptions,
        IOptions<StateSnapshotSettings> stateSnapshotOptions,
        IMapper mapper,
        StateSnapshotPublisher stateSnapshotPublisher)
    {
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions);
        ArgumentNullException.ThrowIfNull(rabbitMQClientOptions.Value);
        ArgumentNullException.ThrowIfNull(stateSnapshotOptions);
        ArgumentNullException.ThrowIfNull(stateSnapshotOptions.Value);
        rabbitMQClientOptions.Value.Validate();
        stateSnapshotOptions.Value.Validate();

        this.logger = logger;
        this.mediator = mediator;
        this.rabbitMQClientSettings = rabbitMQClientOptions.Value;
        this.stateSnapshotSettings = stateSnapshotOptions.Value;
        this.mapper = mapper;
        this.stateSnapshotPublisher = stateSnapshotPublisher;

        var factory = new ConnectionFactory()
        {
            HostName = rabbitMQClientSettings.HostName,
            UserName = rabbitMQClientSettings.UserName,
            Password = rabbitMQClientSettings.Password,
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare(stateSnapshotSettings.ExchangeName, ExchangeType.Direct, durable: false, autoDelete: false);
        channel.QueueDeclare(stateSnapshotSettings.QueueName, durable: false, autoDelete: false);
        foreach (var bindingKey in stateSnapshotSettings.RoutingKeys)
        {
            channel.QueueBind(
                queue: stateSnapshotSettings.QueueName,
                exchange: stateSnapshotSettings.ExchangeName,
                routingKey: bindingKey);
        }

        jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += Consumer_Received;
        channel.BasicConsume(
            queue: stateSnapshotSettings.QueueName,
            autoAck: true,
            consumer: consumer);
    }

    private async void Consumer_Received(object? sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = eventArgs.RoutingKey;
            var request = JsonSerializer.Deserialize<StateSnapshotRequest>(message, jsonSerializerOptions);
            if (request is null) throw new NullReferenceException(nameof(request));

            var getInterestsOutput = await mediator.Send(new Application.Models.Interests.GetInterestsInput { Strategy = request.Strategy });
            var getPositionsOutput = await mediator.Send(new Application.Models.Positions.GetPositionsInput { Strategy = request.Strategy });
            var response = new StateSnapshotResponse
            {
                Positions = getPositionsOutput.Positions.Select(i => mapper.Map<Contracts.Positions.PositionDto>(i)),
                Interests = getInterestsOutput.Interests.Select(i => mapper.Map<Contracts.Interests.InterestDto>(i)),
            };

            stateSnapshotPublisher.PublishEventSnapshot(response, CancellationToken.None);
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

    // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~StateSnapshotConsumer()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }
}
