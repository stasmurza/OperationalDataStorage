using AutoMapper;

namespace MarketDataAggregator.Presentation.Mapping;

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
        CreateMap<Contracts.Ohlcs.AddOhlcRequest, Application.Models.Ohlcs.AddOhlcInput>();
        CreateMap<Contracts.Ohlcs.GetOhlcsRequest, Application.Models.Ohlcs.GetOhlcsInput>();
        CreateMap<Contracts.Ohlcs.TimeInterval, Application.Models.Ohlcs.TimeInterval>();
        CreateMap<Contracts.Interests.GetInterestsRequest, Application.Models.Interests.GetInterestsInput>();
        CreateMap<Contracts.Positions.GetPositionsRequest, Application.Models.Positions.GetPositionsInput>();

        // Models
        CreateMap<Application.Models.Ohlcs.OhlcDto, Contracts.Ohlcs.OhlcDto>();
        CreateMap<Application.Models.Interests.OrderDto, Contracts.Interests.OrderDto>();
        CreateMap<Application.Models.Interests.InterestDto, Contracts.Interests.InterestDto>();
        CreateMap<Application.Models.Positions.OwnTradeDto, Contracts.Positions.OwnTradeDto>();
        CreateMap<Application.Models.Positions.PositionDto, Contracts.Positions.PositionDto>();
    }
}
