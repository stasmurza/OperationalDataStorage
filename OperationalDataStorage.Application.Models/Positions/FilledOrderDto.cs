namespace OperationalDataStorage.Application.Models.Positions;

public class FilledOrderDto
{
    public required Guid OrderId { get; set; }

    public required string Symbol { get; set; }

    public required string Strategy { get; set; }

    public required Direction Direction { get; set; }

    public required decimal FilledQuantity { get; set; }

    public required decimal FinalPositionQuantity { get; set; }

    public required decimal AverageFillPrice { get; set; }
}

