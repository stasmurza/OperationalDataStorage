using EventStore.Infrastructure;
using EventStore.Models.Settings;
using EventStore.Models.Settings.Subscriptions;
using Microsoft.Extensions.Configuration;
using OperationalDataStorage.Service.Tests.Environments;
using OperationalDataStorage.Service.Tests.Proxies;

namespace OperationalDataStorage.Service.Tests.Services;

public sealed class EventStoreMediator : IDisposable
{
    public TestEnvironment TestEnvironment { get; }

    public RabbitMqClientSettings RabbitMqClientSettings { get; }

    public EventsSnapshotSettings EventsSnapshotSettings { get; }

    public TradeRequestSettings TradeRequestSettings { get; }

    public MarketDataSettings MarketDataSettings { get; }

    public MarketOrderSettings MarketOrderSettings { get; }

    public FilledOrderSettings FilledOrderSettings { get; }

    private readonly MessageConsumer<EventStore.Contracts.Events.EventsSnapshot> eventsSnapshotConsumer;
    private readonly MessagePublisher messagePublisher;

    public EventStoreMediator(TestEnvironment testEnvironment)
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
        TradeRequestSettings = TestEnvironment.Configuration.GetSection(nameof(TradeRequestSettings)).Get<TradeRequestSettings>() ??
            throw new NullReferenceException(nameof(TradeRequestSettings));
        MarketDataSettings = TestEnvironment.Configuration.GetSection(nameof(MarketDataSettings)).Get<MarketDataSettings>() ??
            throw new NullReferenceException(nameof(MarketDataSettings));
        MarketOrderSettings = TestEnvironment.Configuration.GetSection(nameof(MarketOrderSettings)).Get<MarketOrderSettings>() ??
            throw new NullReferenceException(nameof(MarketOrderSettings));
        FilledOrderSettings = TestEnvironment.Configuration.GetSection(nameof(FilledOrderSettings)).Get<FilledOrderSettings>() ??
            throw new NullReferenceException(nameof(FilledOrderSettings));
        eventsSnapshotConsumer = new MessageConsumer<EventStore.Contracts.Events.EventsSnapshot>(RabbitMqClientSettings);
        messagePublisher = new MessagePublisher(RabbitMqClientSettings);

        eventsSnapshotConsumer.Subscribe(EventsSnapshotSettings.ExchangeName, EventsSnapshotSettings.RoutingKeys);
    }

    public void Publish<T>(T message, string exchangeName, IEnumerable<string> routingKeys)
    {
        messagePublisher.Publish(message, exchangeName, routingKeys);
    }

    public async Task<EventStore.Contracts.Events.EventsSnapshot> ReadFirstEventsSnapshotAsync(CancellationToken cancellationToken)
    {
        return await eventsSnapshotConsumer.ReadFirstDtoAsync(cancellationToken);
    }

    public void Dispose()
    {
        eventsSnapshotConsumer.Dispose();
    }
}
