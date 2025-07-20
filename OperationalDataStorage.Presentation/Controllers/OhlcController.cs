using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OperationalDataStorage.Application.Models.Ohlcs;
using OperationalDataStorage.Contracts.Ohlcs;
using System.Data;
using System.Net;
using System.Web;

namespace OperationalDataStorage.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class OhlcController(IMediator mediator, IMapper mapper) : ControllerBase
{
    private readonly IMediator mediator = mediator;
    private readonly IMapper mapper = mapper;

    /// <summary>
    /// Returns ohlcs.
    /// </summary>
    /// <param name="symbol" example="BTC/USDT">Symbol.</param>
    /// <param name="granularity" example="Days1">Granularity.</param>
    /// <param name="start" example="2009-06-15T13:45:00">Start date and time, UTC.</param>
    /// <param name="end" example="2009-06-16T13:45:00">End date and time, UTC.</param>
    /// <returns><see cref="GetOhlcsResponse"/></returns>
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetOhlcsResponse))]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<GetOhlcsResponse> GetAsync(
        [FromQuery] string symbol,
        [FromQuery] Contracts.Ohlcs.TimeInterval granularity,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        var input = new GetOhlcsInput
        {
            Symbol = HttpUtility.UrlDecode(symbol),
            Start = start,
            End = end,
            Granularity = mapper.Map<Application.Models.Ohlcs.TimeInterval>(granularity)
        };
        var output = await mediator.Send(input);
        return new GetOhlcsResponse
        {
            Ohlcs = output.Ohlcs.Select(mapper.Map<Contracts.Ohlcs.Ohlc>)
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
