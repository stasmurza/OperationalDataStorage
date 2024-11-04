using AutoMapper;
using MarketDataAggregator.Contracts.Ohlcs;
using MarketDataAggregator.Models.Ohlcs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataAggregator.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MarketDataController(IMediator mediator, IMapper mapper) : ControllerBase
{
    private readonly IMediator mediator = mediator;
    private readonly IMapper mapper = mapper;

    [HttpGet]
    public async Task<GetOhlcsResponse> GetAsync([FromQuery] GetOhlcsRequest request)
    {
        var input = mapper.Map<GetOhlcsInput>(request);
        var output = await mediator.Send(input);
        return new GetOhlcsResponse
        {
            HistoricalExchangeRates = output.HistoricalExchangeRates.Select(mapper.Map<Contracts.Ohlcs.OhlcDto>)
        };
    }

    [HttpPost]
    public async Task PostAsync(AddOhlcRequest request)
    {
        var input = mapper.Map<AddOhlcInput>(request);
        await mediator.Send(input);
    }
}
