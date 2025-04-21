using AutoMapper;

namespace OperationalDataStorage.Presentation.Mapping;

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
        CreateMap<Contracts.Ohlcs.Ohlc, Application.Models.Ohlcs.OhlcInputDto>();
        CreateMap<Contracts.Ohlcs.TimeInterval, Application.Models.Ohlcs.TimeInterval>();
        CreateMap<Contracts.Positions.GetPositionsRequest, Application.Models.Positions.GetPositionsInput>();

        // Models
        CreateMap<Application.Models.Ohlcs.OhlcDto, Contracts.Ohlcs.Ohlc>();
        CreateMap<Application.Models.Positions.OrderDto, Contracts.Positions.Order>();
        CreateMap<Application.Models.Positions.PositionDto, Contracts.Positions.Position>();
    }
}
