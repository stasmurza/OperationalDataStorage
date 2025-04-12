using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Microsoft.Extensions.Configuration;
using OperationalDataStorage.Service.Tests.Containers.Abstractions;
using System.Net;

namespace OperationalDataStorage.Service.Tests.Containers;

public sealed class OperationalDataStorageContainer : ITestContainer
{
    private readonly IFutureDockerImage futureDockerImage;
    private readonly IContainer container;

    public OperationalDataStorageContainer(DockerNetwork dockerNetwork, IConfiguration configuration)
    {
        futureDockerImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
            .WithDockerfile("OperationalDataStorage.Presentation.Dockerfile")
            .WithCleanUp(true)
            .Build();

        using IOutputConsumer outputConsumer = Consume.RedirectStdoutAndStderrToConsole();

        container = new ContainerBuilder()
            .WithName("OperationalDataStorage.Service.Tests.Containers.OperationalDataStorageContainer")
            .WithImage(futureDockerImage)
            .WithEnvironment("DOTNET_ENVIRONMENT", "tests")
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "tests")
            .WithNetwork(dockerNetwork.Network)
            .WithPortBinding(8080, true) // for healhtcheck.
                                         //.WithCreateParameterModifier(i => i.HostConfig.NetworkMode = "host")
            .WithOutputConsumer(outputConsumer)
            .WithCleanUp(true)
            .WithWaitStrategy(
                Wait
                .ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request.ForPort(8080).ForPath("/healthz")
                .ForStatusCode(HttpStatusCode.OK)))
            .Build();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await futureDockerImage.CreateAsync(cancellationToken).ConfigureAwait(false);
        await container.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await container.StopAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await container.DisposeAsync().ConfigureAwait(false);
        await futureDockerImage.DisposeAsync().ConfigureAwait(false);
    }
}