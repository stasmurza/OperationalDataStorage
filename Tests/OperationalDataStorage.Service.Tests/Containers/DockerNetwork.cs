using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;

namespace OperationalDataStorage.Service.Tests.Containers;

public sealed class DockerNetwork : IAsyncDisposable
{
    public INetwork Network { get; }

    public DockerNetwork()
    {
        Network = new NetworkBuilder().WithName(Guid.NewGuid().ToString("D")).Build();
    }

    public async Task CreateAsync(CancellationToken cancellationToken)
    {
        await Network.CreateAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        await Network.DeleteAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await Network.DisposeAsync();
    }
}
