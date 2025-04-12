using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Runtime.CompilerServices;
using EventStore.Models.Settings;
using OperationalDataStorage.Service.Tests.Proxies.Abstractions;

namespace OperationalDataStorage.Service.Tests.Proxies;

public sealed class MessageConsumer<T> : IMessageConsumer
{
    public System.Threading.Channels.Channel<T> Channel { get; }

    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private bool disposedValue;

    public MessageConsumer(RabbitMqClientSettings rabbitMqClientSettings)
    {
        ArgumentNullException.ThrowIfNull(rabbitMqClientSettings);
        rabbitMqClientSettings.Validate();

        var factory = new ConnectionFactory()
        {
            HostName = rabbitMqClientSettings.HostName,
            UserName = rabbitMqClientSettings.UserName,
            Password = rabbitMqClientSettings.Password,
            Port = rabbitMqClientSettings.Port,
            VirtualHost = "/"
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        Channel = System.Threading.Channels.Channel.CreateUnbounded<T>();
    }

    public void Subscribe(string exchangeName, IEnumerable<string> routingKeys)
    {
        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
        var queueDeclareResult = channel.QueueDeclare(durable: false, autoDelete: true);
        string queueName = queueDeclareResult.QueueName;
        foreach (var bindingKey in routingKeys)
        {
            channel.QueueBind(
                exchange: exchangeName,
                queue: queueName,
                routingKey: bindingKey);
        }

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += Consumer_Received;
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    public async IAsyncEnumerable<T> GetDtosAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var @object in Channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return @object;
        }
    }

    public async Task<T> ReadFirstDtoAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (Channel.Reader.TryRead(out var item))
            {
                return item;
            }

            await Channel.Reader.WaitToReadAsync(cancellationToken);
        }

        throw new InvalidOperationException("Sequence contains no elements");
    }

    public void Dispose()
    {
        if (!disposedValue)
        {
            // Free unmanaged resources (unmanaged objects) and override finalizer
            // Set large fields to null
            channel.Close();
            connection.Close();
            channel.Dispose();
            connection.Dispose();
            disposedValue = true;
        }
    }

    private async void Consumer_Received(object? sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = eventArgs.RoutingKey;
            var @object = JsonSerializer.Deserialize<T>(message, jsonSerializerOptions);
            if (@object is null) throw new NullReferenceException(nameof(@object));

            // event or queue
            await Channel.Writer.WriteAsync(@object);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}
