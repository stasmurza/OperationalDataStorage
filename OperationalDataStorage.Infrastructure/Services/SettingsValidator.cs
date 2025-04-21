using Microsoft.Extensions.Options;
using OperationalDataStorage.Infrastructure.Models.Settings;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq.Consumers.Subscriptions;

namespace EventStore.Core.Services;

public class SettingsValidator
{
    private readonly DatabaseSettings databaseSettings;
    private readonly EventsSnapshotSettings eventsSnapshotSettings;
    private readonly RabbitMqClientSettings rabbitMqClientSettings;

    public SettingsValidator(
        IOptions<DatabaseSettings> databaseOptions,
        IOptions<EventsSnapshotSettings> eventsSnapshotOptions,
        IOptions<RabbitMqClientSettings> rabbitMqClientOptions)
    {
        ArgumentNullException.ThrowIfNull(databaseOptions.Value);
        ArgumentNullException.ThrowIfNull(eventsSnapshotOptions.Value);
        ArgumentNullException.ThrowIfNull(rabbitMqClientOptions.Value);

        databaseSettings = databaseOptions.Value;
        eventsSnapshotSettings = eventsSnapshotOptions.Value;
        rabbitMqClientSettings = rabbitMqClientOptions.Value;
    }

    public void Validate()
    {
        databaseSettings.Validate();
        eventsSnapshotSettings.Validate();
        rabbitMqClientSettings.Validate();
    }
}
