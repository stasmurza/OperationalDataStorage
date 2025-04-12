using Microsoft.Extensions.Configuration;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq.Consumers.Subscriptions;
using OperationalDataStorage.Service.Tests.Environments;
using OperationalDataStorage.Service.Tests.Proxies;

namespace OperationalDataStorage.Service.Tests.Services;

public sealed class EventStoreMediator : IDisposable
{
    public TestEnvironment TestEnvironment { get; }

    public RabbitMqClientSettings RabbitMqClientSettings { get; }

    public EventsSnapshotSettings EventsSnapshotSettings { get; }

    private readonly MessageConsumer<Contracts.Events.EventsSnapshot> eventsSnapshotConsumer;
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

        eventsSnapshotConsumer = new MessageConsumer<Contracts.Events.EventsSnapshot>(RabbitMqClientSettings);
        messagePublisher = new MessagePublisher(RabbitMqClientSettings);

        eventsSnapshotConsumer.Subscribe(EventsSnapshotSettings.ExchangeName, EventsSnapshotSettings.RoutingKeys);
    }

    public void Publish<T>(T message, string exchangeName, IEnumerable<string> routingKeys)
    {
        messagePublisher.Publish(message, exchangeName, routingKeys);
    }

    public async Task<Contracts.Events.EventsSnapshot> ReadFirstEventsSnapshotAsync(CancellationToken cancellationToken)
    {
        return await eventsSnapshotConsumer.ReadFirstDtoAsync(cancellationToken);
    }

    public void Dispose()
    {
        eventsSnapshotConsumer.Dispose();
    }
}
