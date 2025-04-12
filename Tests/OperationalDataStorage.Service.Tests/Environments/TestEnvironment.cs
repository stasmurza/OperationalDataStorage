using Microsoft.Extensions.Configuration;
using OperationalDataStorage.Service.Tests.Containers;
using OperationalDataStorage.Service.Tests.Settings;

namespace OperationalDataStorage.Service.Tests.Environments;

public sealed class TestEnvironment : IAsyncDisposable
{
    public TestsSettings TestsSettings { get; } = new();

    public IConfiguration Configuration { get; }

    public DockerNetwork DockerNetwork { get; }

    public RabbitMqContainer RabbitMqContainer { get; }

    public DatabaseContainer DatabaseContainer { get; }

    public OperationalDataStorageContainer EventStoreContainer { get; }

    public TestEnvironment()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.tests.json", optional: false, reloadOnChange: false);

        Configuration = builder.Build();

        DockerNetwork = new DockerNetwork();
        RabbitMqContainer = new RabbitMqContainer(DockerNetwork, Configuration);
        DatabaseContainer = new DatabaseContainer(DockerNetwork, Configuration);
        EventStoreContainer = new OperationalDataStorageContainer(DockerNetwork, Configuration);
    }

    public async Task SetupAsync(CancellationToken cancellationToken)
    {
        try
        {
            await DockerNetwork.CreateAsync(cancellationToken).ConfigureAwait(false);
            await RabbitMqContainer.StartAsync(cancellationToken).ConfigureAwait(false);
            await DatabaseContainer.StartAsync(cancellationToken).ConfigureAwait(false);
            await EventStoreContainer.StartAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.ToString());
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await EventStoreContainer.StopAsync(cancellationToken).ConfigureAwait(false);
        await DatabaseContainer.StopAsync(cancellationToken).ConfigureAwait(false);
        await RabbitMqContainer.StopAsync(cancellationToken).ConfigureAwait(false);
        await DockerNetwork.DeleteAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await EventStoreContainer.DisposeAsync().ConfigureAwait(false);
        await DatabaseContainer.DisposeAsync().ConfigureAwait(false);
        await RabbitMqContainer.DisposeAsync().ConfigureAwait(false);
        await DockerNetwork.DisposeAsync().ConfigureAwait(false);
    }
}
