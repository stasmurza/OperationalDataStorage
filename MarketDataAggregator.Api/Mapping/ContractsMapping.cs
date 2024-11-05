using AutoMapper;
using MarketDataAggregator.Contracts.Ohlcs;
using MarketDataAggregator.Models.Ohlcs;

namespace MarketDataAggregator.Api.Mapping;

/// <summary>
/// Auto mapper types mapping.
/// </summary>
public class ContractsMapping : Profile
{
    /// <summary>
    /// Map contracts.
    /// </summary>
    public ContractsMapping()
    {
        CreateMap<AddOhlcRequest, AddOhlcsInput>();
        CreateMap<GetOhlcsRequest, GetOhlcsInput>();
        CreateMap<Models.Ohlcs.OhlcDto, OhlcDto>();
        CreateMap<Contracts.TimeInterval, TimeInterval>();
    }
}
