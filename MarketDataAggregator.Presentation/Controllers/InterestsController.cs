using AutoMapper;
using MarketDataAggregator.Contracts.Interests;
using MarketDataAggregator.Application.Models.Interests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataAggregator.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class InterestsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    private readonly IMediator mediator = mediator;
    private readonly IMapper mapper = mapper;

    [HttpGet]
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
