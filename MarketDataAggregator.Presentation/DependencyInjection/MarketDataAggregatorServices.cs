using MarketDataAggregator.Application.Ohlcs;
using MarketDataAggregator.Application.Repositories.Abstractions;
using MarketDataAggregator.Persistence.Repositories;
using MarketDataAggregator.Infrastructure;
using MarketDataAggregator.Infrastructure.Settings;
using MarketDataAggregator.Infrastructure.Settings.RabbitMq;
using Microsoft.OpenApi.Models;
using MarketDataAggregator.Domain.Entities.Positions;
using MarketDataAggregator.Domain.Entities.Ohlcs;
using MarketDataAggregator.Domain.Entities.Interests;
using MarketDataAggregator.Infrastructure.Persistence;
using MarketDataAggregator.Infrastructure.Settings.RabbitMq.Consumers.Subscriptions;
using MarketDataAggregator.Infrastructure.Persistence.Repositories;

namespace MarketDataAggregator.Presentation.DependencyInjection;

/// <summary>
/// Core layer injection.
/// </summary>
public static class MarketDataAggregatorServices
{
    /// <summary>
    /// Add core layer services.
    /// </summary>
    /// <param name="services"></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddMarketDataAggregatorServices(this IServiceCollection services, IConfiguration configuration)
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
            var filePath = Path.Combine(AppContext.BaseDirectory, "MarketDataAggregator.Contracts.xml");
            c.IncludeXmlComments(filePath);
            c.DescribeAllParametersInCamelCase();
        });

        return services;
    }
}
