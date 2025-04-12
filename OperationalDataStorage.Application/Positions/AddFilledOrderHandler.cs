using OperationalDataStorage.Application.Positions.Extensions;
using OperationalDataStorage.Application.Repositories.Abstractions;
using OperationalDataStorage.Application.Models.Positions;
using MediatR;
using OperationalDataStorage.Domain.Entities.Positions;

namespace OperationalDataStorage.Application.Positions;

public class AddFilledOrderHandler(IRepository<Position> positionRepository) : IRequestHandler<AddFilledOrderInput>
{
    public async Task Handle(AddFilledOrderInput input, CancellationToken cancellationToken)
    {
        foreach (var dto in input.Dtos) await AddFilledOrderAsync(dto, cancellationToken);
    }

    public async Task AddFilledOrderAsync(FilledOrderDto dto, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;

        var entity = await positionRepository.FirstOrDefaultAsync(i => i.Symbol == dto.Symbol && i.Strategy == dto.Strategy);
        if (entity is null)
        {
            entity = dto.ToPositionEntity();
            await positionRepository.CreateAsync(entity);
        }
        else
        {
            entity.Apply(dto);
            if (entity.Quantity == 0) await positionRepository.DeleteAsync(entity.Id);
            else await positionRepository.UpdateAsync(entity);
        }
    }
}
