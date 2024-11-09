using AutoMapper;
using MarketDataAggregator.Contracts.Ohlcs;
using MarketDataAggregator.Contracts.Positions;
using MarketDataAggregator.Application.Models.Positions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MarketDataAggregator.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class PositionsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    private readonly IMediator mediator = mediator;
    private readonly IMapper mapper = mapper;

    /// <summary>
    /// Returns positions.
    /// </summary>
    /// <param name="request"><see cref="GetPositionsRequest"/></param>
    /// <returns><see cref="GetPositionsResponse"/></returns>
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetOhlcsResponse))]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
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
