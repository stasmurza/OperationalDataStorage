using AutoMapper;
using MarketDataAggregator.Contracts.Ohlcs;
using MarketDataAggregator.Contracts.Positions;
using MarketDataAggregator.Models.Positions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataAggregator.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PositionsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    private readonly IMediator mediator = mediator;
    private readonly IMapper mapper = mapper;

    [HttpGet]
    public async Task<GetPositionsResponse> GetAsync([FromQuery] GetPositionsRequest request)
    {
        var input = mapper.Map<GetPositionsInput>(request);
        var output = await mediator.Send(input);
        return new GetPositionsResponse
        {
            Positions = output.Positions.Select(mapper.Map<Contracts.Positions.PositionDto>)
        };
    }
}
