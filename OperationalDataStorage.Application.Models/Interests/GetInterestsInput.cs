using OperationalDataStorage.Application.Models.Ohlcs;
using MediatR;

namespace OperationalDataStorage.Application.Models.Interests;

public class GetInterestsInput : IRequest<GetInterestsOutput>
{
    public required string Strategy { get; set; }
}
