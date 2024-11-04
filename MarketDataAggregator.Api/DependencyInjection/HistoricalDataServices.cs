using MarketDataAggregator.Api.HostedServices;
using MarketDataAggregator.Core.Ohlcs;
using MarketDataAggregator.Core.Ohlcs.Aggregates;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Data;
using MarketDataAggregator.Data.Repositories;
using MarketDataAggregator.Models.Entities.Events;
using MarketDataAggregator.Models.Entities.Ohlcs;
using MarketDataAggregator.Models.Options;

namespace MarketDataAggregator.Api.DependencyInjection;

/// <summary>
/// Core layer injection.
/// </summary>
public static class HistoricalDataServices
{
    /// <summary>
    /// Add core layer services.
    /// </summary>
    /// <param name="services"></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddHistoricalDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetOhlcsHandler).Assembly));
        services.AddAutoMapperService();
        services.Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)));
        services.AddSingleton<IContext, DbContext>();
        services.AddSingleton<IRepository<Event>, EventRepository>();
        services.AddSingleton<IRepository<Ohlc>, OhlcRepository>();
        services.AddSingleton<OhlcAggregateBuilder>();
        services.AddSingleton<OhlcAggregateBuilder>();
        services.AddHostedService<OhlcAggregator>();

        return services;
    }
}
