using System.Text;
using Newtonsoft.Json;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq;
using OperationalDataStorage.Service.Tests.Proxies.Abstractions;
using RabbitMQ.Client;

namespace OperationalDataStorage.Service.Tests.Proxies;

public sealed class MessagePublisher : IMessagePublisher
{
    private readonly RabbitMqClientSettings rabbitMqClientSettings;

    public MessagePublisher(RabbitMqClientSettings rabbitMqClientSettings)
    {
        ArgumentNullException.ThrowIfNull(rabbitMqClientSettings);
        rabbitMqClientSettings.Validate();

        this.rabbitMqClientSettings = rabbitMqClientSettings;
    }

    public void Publish<T>(T message, string exchangeName, IEnumerable<string> routingKeys)
    {
        var factory = new ConnectionFactory()
        {
            HostName = rabbitMqClientSettings.HostName,
            UserName = rabbitMqClientSettings.UserName,
            Password = rabbitMqClientSettings.Password,
            Port = rabbitMqClientSettings.Port,
            VirtualHost = "/"
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: false, autoDelete: false);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        foreach (var key in routingKeys)
        {
            channel.BasicPublish(
                exchange: exchangeName,
                routingKey: key,
                basicProperties: null,
                body: body);
        }
    }

    public void Publish<T>(IEnumerable<T> messages, string exchangeName, IEnumerable<string> routingKeys)
    {
        var factory = new ConnectionFactory()
        {
            HostName = rabbitMqClientSettings.HostName,
            UserName = rabbitMqClientSettings.UserName,
            Password = rabbitMqClientSettings.Password,
            Port = rabbitMqClientSettings.Port,
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: false, autoDelete: true);

        foreach (var message in messages)
        {
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            foreach (var key in routingKeys)
            {
                channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: key,
                    basicProperties: null,
                    body: body);
            }
        }
    }
}
