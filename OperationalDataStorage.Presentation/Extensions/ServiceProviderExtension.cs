using EventStore.Core.Services;
using Microsoft.Extensions.Options;
using OperationalDataStorage.Infrastructure.Models.Settings;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq.Consumers.Subscriptions;
using System.Text.Json;

namespace OperationalDataStorage.Presentation.Extensions;

public static class ServiceProviderExtension
{
    public static IServiceProvider AddLifetimeLogger(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("StartupLogger");
        var hostApplicationLifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();

        hostApplicationLifetime.ApplicationStarted.Register(() => logger.LogInformation("The application has started."));
        hostApplicationLifetime.ApplicationStopping.Register(() => logger.LogInformation("The application is stopping."));
        hostApplicationLifetime.ApplicationStopped.Register(() => logger.LogInformation("The application has stopped."));

        return services;
    }

    public static IServiceProvider ValidateSettings(this IServiceProvider services)
    {
        using (var scope = services.CreateScope())
        {
            var settingsValidator = scope.ServiceProvider.GetRequiredService<SettingsValidator>();
            settingsValidator.Validate();
        }

        return services;
    }

    public static IServiceProvider LogSettings(this IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<IServiceProvider>>();
        var databaseSettings = services.GetRequiredService<IOptions<DatabaseSettings>>();
        var eventsSnapshotSettings = services.GetRequiredService<IOptions<EventsSnapshotSettings>>();
        var rabbitMqClientSettings = services.GetRequiredService<IOptions<RabbitMqClientSettings>>();
        var hostEnvironment = services.GetRequiredService<IHostEnvironment>();

        logger.LogInformation("Environment: {environment}", hostEnvironment.EnvironmentName);
        logger.LogInformation("Database settings: {databaseSettings}", JsonSerializer.Serialize(databaseSettings));
        logger.LogInformation("{settingsName}: {settings}", nameof(eventsSnapshotSettings), JsonSerializer.Serialize(eventsSnapshotSettings));
        logger.LogInformation("{settingsName}: {settings}", nameof(rabbitMqClientSettings), JsonSerializer.Serialize(rabbitMqClientSettings));

        return services;
    }
}
