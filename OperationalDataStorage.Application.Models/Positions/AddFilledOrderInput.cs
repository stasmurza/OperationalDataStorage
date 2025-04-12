using MediatR;

namespace OperationalDataStorage.Application.Models.Positions;

public class AddFilledOrderInput: IRequest
{
    public required IEnumerable<FilledOrderDto> Dtos { get; set; }
}
