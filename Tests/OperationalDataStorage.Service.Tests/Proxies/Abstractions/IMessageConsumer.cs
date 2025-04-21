namespace OperationalDataStorage.Service.Tests.Proxies.Abstractions;

public interface IMessageConsumer : IDisposable
{
    public void Subscribe(string exchangeName, IEnumerable<string> routingKeys);
}
