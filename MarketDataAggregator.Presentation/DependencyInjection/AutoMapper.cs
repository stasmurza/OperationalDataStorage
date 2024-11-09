using AutoMapper;
using MarketDataAggregator.Presentation.Mapping;

namespace MarketDataAggregator.Presentation.DependencyInjection;

public static class AutoMapper
{
    /// <summary>
    /// Inject Automapper
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAutoMapperService(this IServiceCollection services)
    {
        services.AddSingleton<Profile, ClassesMapping>();

        services.AddSingleton(sp => new MapperConfiguration(cfg =>
        {
            cfg.AddProfiles(sp.GetServices<Profile>());
        }).CreateMapper());

        return services;
    }
}
