using OperationalDataStorage.Application.Ohlcs.Extensions;
using OperationalDataStorage.Application.Repositories.Abstractions;
using OperationalDataStorage.Application.Models.Ohlcs;
using MediatR;
using OperationalDataStorage.Domain.Entities.Ohlcs;

namespace OperationalDataStorage.Application.Ohlcs;

public class GetOhlcsHandler(IRepository<Ohlc> ohlcRepository) : IRequestHandler<GetOhlcsInput, GetOhlcsOutput>
{
    private readonly IRepository<Ohlc> ohlcRepository = ohlcRepository;

    public async Task<GetOhlcsOutput> Handle(GetOhlcsInput input, CancellationToken cancellationToken)
    {
        var interval = Enum.Parse<Domain.Entities.Ohlcs.TimeInterval>(input.Granularity.ToString());
        var ohlcs = await ohlcRepository.GetAsync(i =>
            i.Symbol == input.Symbol &&
            i.Interval == interval &&
            i.StartTime >= input.Start &&
            i.StartTime <= input.End);

        return new GetOhlcsOutput()
        {
            Ohlcs = ohlcs.Select(i => i.ToDto()),
        };
    }
}