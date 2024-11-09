using AutoMapper;
using MarketDataAggregator.Contracts.Interests;
using MarketDataAggregator.Application.Models.Interests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MarketDataAggregator.Contracts.Ohlcs;
using System.Net;

namespace MarketDataAggregator.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class InterestsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    private readonly IMediator mediator = mediator;
    private readonly IMapper mapper = mapper;

    /// <summary>
    /// Returns interests.
    /// </summary>
    /// <param name="request"><see cref="GetInterestsRequest"/></param>
    /// <returns><see cref="GetInterestsResponse"/></returns>
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetOhlcsResponse))]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<GetInterestsResponse> GetAsync([FromQuery] GetInterestsRequest request)
    {
        var input = mapper.Map<GetInterestsInput>(request);
        var output = await mediator.Send(input);
        return new GetInterestsResponse
        {
            Interests = output.Interests.Select(mapper.Map<Contracts.Interests.InterestDto>)
        };
    }
}
