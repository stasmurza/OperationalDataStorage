using MarketDataAggregator.Api.HostedServices;
using MarketDataAggregator.Core.Ohlcs;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Data;
using MarketDataAggregator.Data.Repositories;
using MarketDataAggregator.Infrastructure.Settings;

namespace MarketDataAggregator.Api.DependencyInjection;

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
        services.AddSingleton<IContext, DbContext>();
        services.AddSingleton<IRepository<Event>, EventRepository>();
        services.AddSingleton<IRepository<Ohlc>, OhlcRepository>();
        services.AddSingleton<Core.Ohlcs.OhlcAggregator>();
        services.AddSingleton<Core.Ohlcs.OhlcAggregator>();
        services.AddHostedService<HostedServices.OhlcAggregator>();

        return services;
    }
}
