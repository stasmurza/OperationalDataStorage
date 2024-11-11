using OperationalDataStorage.Application.Interests.Extensions;
using OperationalDataStorage.Application.Repositories.Abstractions;
using OperationalDataStorage.Application.Models.Interests;
using MediatR;
using OperationalDataStorage.Domain.Entities.Interests;

namespace OperationalDataStorage.Application.Interests;

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
