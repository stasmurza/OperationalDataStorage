using MarketDataAggregator.Core.Interests.Extensions;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Interests;
using MarketDataAggregator.Models.Interests;
using MediatR;

namespace MarketDataAggregator.Core.Interests;

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
