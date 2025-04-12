using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Configuration;
using OperationalDataStorage.Infrastructure.Models.Settings;
using OperationalDataStorage.Service.Tests.Containers.Abstractions;
using Testcontainers.MongoDb;

namespace OperationalDataStorage.Service.Tests.Containers;

public sealed class DatabaseContainer : ITestContainer
{
    public string Host { get; }

    private readonly MongoDbContainer container;
    private readonly DatabaseSettings databaseSettings;

    public DatabaseContainer(DockerNetwork dockerNetwork, IConfiguration configuration)
    {
        databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>() ?? throw new NullReferenceException(nameof(DatabaseSettings));

        container = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .WithName(databaseSettings.HostName)
            .WithUsername("admin")
            .WithPassword("admin")
            .WithNetwork(dockerNetwork.Network)
            .WithPortBinding(databaseSettings.Port, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(databaseSettings.Port))
            .WithCleanUp(true)
            .Build();

        Host = "localhost";
    }

    public async Task StartAsync(CancellationToken cancellationToken) => await container.StartAsync(cancellationToken).ConfigureAwait(false);

    public async Task StopAsync(CancellationToken cancellationToken) => await container.StopAsync(cancellationToken).ConfigureAwait(false);

    public async ValueTask DisposeAsync() => await container.DisposeAsync();

    public int GetMappedPort() => container.GetMappedPublicPort(databaseSettings.Port);

    public string GetConnectionString()
    {
        return container.GetConnectionString();
    }
}