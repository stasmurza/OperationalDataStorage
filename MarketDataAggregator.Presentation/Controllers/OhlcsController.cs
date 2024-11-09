using AutoMapper;
using MarketDataAggregator.Contracts.Ohlcs;
using MarketDataAggregator.Application.Models.Ohlcs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace MarketDataAggregator.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class OhlcsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    private readonly IMediator mediator = mediator;
    private readonly IMapper mapper = mapper;

    /// <summary>
    /// Returns ohlcs.
    /// </summary>
    /// <param name="request"><see cref="GetOhlcsRequest"/></param>
    /// <returns><see cref="GetOhlcsResponse"/></returns>
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetOhlcsResponse))]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<GetOhlcsResponse> GetAsync([FromQuery] GetOhlcsRequest request)
    {
        var input = mapper.Map<GetOhlcsInput>(request);
        var output = await mediator.Send(input);
        return new GetOhlcsResponse
        {
            Ohlcs = output.Ohlcs.Select(mapper.Map<Contracts.Ohlcs.OhlcDto>)
        };
    }

    /// <summary>
    /// Creates or updates OHLC item and returns created company's data.
    /// </summary>
    /// <param name="request">OHLC <see cref="AddOhlcRequest"/></param>
    /// <returns>Id of created or updated item <see cref="AddOhlcResponse"/></returns>
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] AddOhlcRequest request)
    {
        var input = mapper.Map<AddOhlcInput>(request);
        var output = await mediator.Send(input);

        return StatusCode(
            (int)HttpStatusCode.Created,
            mapper.Map<AddOhlcResponse>(output));
    }
}
