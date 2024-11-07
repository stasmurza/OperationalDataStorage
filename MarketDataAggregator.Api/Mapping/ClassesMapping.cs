using AutoMapper;

namespace MarketDataAggregator.Api.Mapping;

/// <summary>
/// Auto mapper types mapping.
/// </summary>
public class ClassesMapping : Profile
{
    /// <summary>
    /// Map contracts.
    /// </summary>
    public ClassesMapping()
    {
        // Contract
        CreateMap<Contracts.Ohlcs.AddOhlcRequest, Models.Ohlcs.AddOhlcInput>();
        CreateMap<Contracts.Ohlcs.GetOhlcsRequest, Models.Ohlcs.GetOhlcsInput>();
        CreateMap<Contracts.Ohlcs.TimeInterval, Models.Ohlcs.TimeInterval>();
        CreateMap<Contracts.Interests.GetInterestsRequest, Models.Interests.GetInterestsInput>();
        CreateMap<Contracts.Positions.GetPositionsRequest, Models.Positions.GetPositionsInput>();

        // Models
        CreateMap<Models.Ohlcs.OhlcDto, Contracts.Ohlcs.OhlcDto>();
        CreateMap<Models.Interests.OrderDto, Contracts.Interests.OrderDto>();
        CreateMap<Models.Interests.InterestDto, Contracts.Interests.InterestDto>();
        CreateMap<Models.Positions.OwnTradeDto, Contracts.Positions.OwnTradeDto>();
        CreateMap<Models.Positions.PositionDto, Contracts.Positions.PositionDto>();
    }
}
