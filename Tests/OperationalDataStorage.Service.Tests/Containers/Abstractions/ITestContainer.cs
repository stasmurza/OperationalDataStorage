namespace OperationalDataStorage.Service.Tests.Containers.Abstractions;

public interface ITestContainer : IAsyncDisposable
{
    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
