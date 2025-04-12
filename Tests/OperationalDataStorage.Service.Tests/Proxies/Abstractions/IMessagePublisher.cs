namespace OperationalDataStorage.Service.Tests.Proxies.Abstractions;

public interface IMessagePublisher
{
    void Publish<T>(T message, string exchangeName, IEnumerable<string> routingKeys);

    void Publish<T>(IEnumerable<T> messages, string exchangeName, IEnumerable<string> routingKeys);
}
