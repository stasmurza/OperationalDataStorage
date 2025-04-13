using Microsoft.Extensions.Configuration;
using OperationalDataStorage.Contracts.Ohlcs;
using OperationalDataStorage.Domain.Entities.Ohlcs;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq.Consumers.Subscriptions;
using OperationalDataStorage.Service.Tests.Environments;
using OperationalDataStorage.Service.Tests.Proxies;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OperationalDataStorage.Service.Tests.Services;

public sealed class OperationalDataStorageMediator : IDisposable
{
    public TestEnvironment TestEnvironment { get; }

    public RabbitMqClientSettings RabbitMqClientSettings { get; }

    public EventsSnapshotSettings EventsSnapshotSettings { get; }

    private readonly MessageConsumer<Contracts.Events.EventsSnapshot> eventsSnapshotConsumer;
    private readonly MessagePublisher messagePublisher;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public OperationalDataStorageMediator(TestEnvironment testEnvironment)
    {
        TestEnvironment = testEnvironment;

        RabbitMqClientSettings = new RabbitMqClientSettings
        {
            HostName = TestEnvironment.RabbitMqContainer.Host,
            UserName = TestEnvironment.RabbitMqContainer.UserName,
            Password = TestEnvironment.RabbitMqContainer.Password,
            Port = TestEnvironment.RabbitMqContainer.GetMappedPort(),
            ManagementPort = TestEnvironment.RabbitMqContainer.GetMappedPort(),
        };

        EventsSnapshotSettings = TestEnvironment.Configuration.GetSection(nameof(EventsSnapshotSettings)).Get<EventsSnapshotSettings>() ??
            throw new NullReferenceException(nameof(EventsSnapshotSettings));

        eventsSnapshotConsumer = new MessageConsumer<Contracts.Events.EventsSnapshot>(RabbitMqClientSettings);
        messagePublisher = new MessagePublisher(RabbitMqClientSettings);

        eventsSnapshotConsumer.Subscribe(EventsSnapshotSettings.ExchangeName, EventsSnapshotSettings.RoutingKeys);

        jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public void Publish<T>(T message, string exchangeName, IEnumerable<string> routingKeys)
    {
        messagePublisher.Publish(message, exchangeName, routingKeys);
    }

    public async Task<Contracts.Events.EventsSnapshot> ReadFirstEventsSnapshotAsync(CancellationToken cancellationToken)
    {
        return await eventsSnapshotConsumer.ReadFirstDtoAsync(cancellationToken);
    }

    public async Task<GetOhlcsResponse> GetOhlcsAsync(
        string symbol,
        DateTime start,
        DateTime end,
        Contracts.Ohlcs.TimeInterval granularity,
        CancellationToken cancellationToken)
    {
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://OperationalDataStorage.Service.Tests.Containers.OperationalDataStorageContainer:8080"),
        };

        using HttpResponseMessage response = await httpClient.GetAsync($"todos/{symbol}/{start:0}/{end:0}/{granularity}", cancellationToken);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonSerializer.Deserialize<GetOhlcsResponse>(jsonResponse, jsonSerializerOptions);
    }

    public void Dispose()
    {
        eventsSnapshotConsumer.Dispose();
    }
}
