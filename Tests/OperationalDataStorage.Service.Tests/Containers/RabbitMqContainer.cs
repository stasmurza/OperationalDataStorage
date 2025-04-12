using DotNet.Testcontainers.Builders;
using EventStore.Models.Settings;
using Microsoft.Extensions.Configuration;
using OperationalDataStorage.Service.Tests.Containers.Abstractions;
using Testcontainers.RabbitMq;

namespace OperationalDataStorage.Service.Tests.Containers;

public sealed class RabbitMqContainer : ITestContainer
{
    public string Host { get; }

    public string UserName { get; }

    public string Password { get; }

    private readonly Testcontainers.RabbitMq.RabbitMqContainer container;
    private readonly RabbitMqClientSettings rabbitMqClientSettings;

    public RabbitMqContainer(DockerNetwork dockerNetwork, IConfiguration configuration)
    {
        rabbitMqClientSettings = configuration.GetSection(nameof(RabbitMqClientSettings)).Get<RabbitMqClientSettings>() ?? throw new NullReferenceException(nameof(RabbitMqClientSettings));

        container = new RabbitMqBuilder()
            .WithImage("rabbitmq:3.11-management")
            .WithNetwork(dockerNetwork.Network)
            .WithName(rabbitMqClientSettings.HostName)
            .WithEnvironment("RABBITMQ_DEFAULT_USER", rabbitMqClientSettings.UserName)
            .WithEnvironment("RABBITMQ_DEFAULT_PASS", rabbitMqClientSettings.Password)
            .WithCleanUp(true)
            .WithPortBinding(rabbitMqClientSettings.Port, rabbitMqClientSettings.Port)
            .WithPortBinding(rabbitMqClientSettings.ManagementPort, rabbitMqClientSettings.ManagementPort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Server startup complete"))
            .Build();

        Host = "localhost";
        UserName = rabbitMqClientSettings.UserName;
        Password = rabbitMqClientSettings.Password;
    }

    public async Task StartAsync(CancellationToken cancellationToken) => await container.StartAsync(cancellationToken).ConfigureAwait(false);

    public async Task StopAsync(CancellationToken cancellationToken) => await container.StopAsync(cancellationToken).ConfigureAwait(false);

    public async ValueTask DisposeAsync() => await container.DisposeAsync();

    public int GetMappedPort() => container.GetMappedPublicPort(rabbitMqClientSettings.Port);

    public int GetMappedManagementPort() => container.GetMappedPublicPort(rabbitMqClientSettings.ManagementPort);
}
