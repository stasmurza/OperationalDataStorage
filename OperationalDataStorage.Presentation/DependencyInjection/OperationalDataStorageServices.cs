using OperationalDataStorage.Application.Ohlcs;
using OperationalDataStorage.Application.Repositories.Abstractions;
using OperationalDataStorage.Persistence.Repositories;
using OperationalDataStorage.Infrastructure;
using Microsoft.OpenApi.Models;
using OperationalDataStorage.Domain.Entities.Positions;
using OperationalDataStorage.Domain.Entities.Ohlcs;
using OperationalDataStorage.Domain.Entities.Interests;
using OperationalDataStorage.Infrastructure.Persistence;
using OperationalDataStorage.Infrastructure.Persistence.Repositories;
using OperationalDataStorage.Infrastructure.Models.Settings;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq;
using OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq.Consumers.Subscriptions;

namespace OperationalDataStorage.Presentation.DependencyInjection;

/// <summary>
/// Core layer injection.
/// </summary>
public static class OperationalDataStorageServices
{
    /// <summary>
    /// Add core layer services.
    /// </summary>
    /// <param name="services"></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddOperationalDataStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetOhlcsHandler).Assembly));
        services.AddAutoMapperService();
        services.Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)));
        services.Configure<RabbitMqClientSettings>(configuration.GetSection(nameof(RabbitMqClientSettings)));
        services.Configure<EventsSnapshotSettings>(configuration.GetSection(nameof(EventsSnapshotSettings)));
        services.AddSingleton<IContext, DbContext>();
        services.AddSingleton<IRepository<Interest>, InterestRepository>();
        services.AddSingleton<IRepository<Ohlc>, OhlcRepository>();
        services.AddSingleton<IRepository<Position>, PositionRepository>();
        services.AddSingleton<EventsSnapshotConsumer>();
        services.AddHostedService<HostedServices.MessageConsumersHostedService>();

        return services;
    }

    /// <summary>
    /// Configure Open API.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Market data aggregator.",
                Description = "Aggregator of market data."
            });
            var filePath = Path.Combine(AppContext.BaseDirectory, "OperationalDataStorage.Contracts.xml");
            c.IncludeXmlComments(filePath);
            c.DescribeAllParametersInCamelCase();
        });

        return services;
    }
}
