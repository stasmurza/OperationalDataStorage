using MarketDataAggregator.Application.Interests.Extensions;
using MarketDataAggregator.Application.Repositories.Abstractions;
using MarketDataAggregator.Application.Models.Interests;
using MediatR;
using MarketDataAggregator.Domain.Entities.Interests;

namespace MarketDataAggregator.Application.Interests;

public class GetInterestsHandler(IRepository<Interest> interestRepository) : IRequestHandler<GetInterestsInput, GetInterestsOutput>
{
    private readonly IRepository<Interest> interestRepository = interestRepository;

    public async Task<GetInterestsOutput> Handle(GetInterestsInput input, CancellationToken cancellationToken)
    {
        var interests = await interestRepository.GetAsync(i => i.Strategy == input.Strategy);

        return new GetInterestsOutput()
        {
            Interests = interests.Select(i => i.ToDto()),
        };
    }
}
