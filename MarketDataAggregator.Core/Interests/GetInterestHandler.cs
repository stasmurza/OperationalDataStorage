using MarketDataAggregator.Core.Interests.Extensions;
using MarketDataAggregator.Core.Repositories.Abstractions;
using MarketDataAggregator.Entities.Interests;
using MarketDataAggregator.Models.Interests;
using MediatR;

namespace MarketDataAggregator.Core.Interests;

public class GetInterestHandler(IRepository<Interest> interestRepository) : IRequestHandler<GetInterestInput, GetInterestOutput>
{
    private readonly IRepository<Interest> interestRepository = interestRepository;

    public async Task<GetInterestOutput> Handle(GetInterestInput input, CancellationToken cancellationToken)
    {
        var interests = await interestRepository.GetAsync(i => i.Strategy == input.Strategy);

        return new GetInterestOutput()
        {
            Interests = interests.Select(i => i.ToDto()),
        };
    }
}
