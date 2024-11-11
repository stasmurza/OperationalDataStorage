using MediatR;

namespace OperationalDataStorage.Application.Models.Positions;

public class GetPositionsInput : IRequest<GetPositionsOutput>
{
    public required string Strategy { get; set; }
}
