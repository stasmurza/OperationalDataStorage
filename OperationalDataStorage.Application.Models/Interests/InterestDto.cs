namespace OperationalDataStorage.Application.Models.Interests;

public class InterestDto
{
    public required string Id { get; set; }

    public required string Symbol { get; set; }

    public required string Strategy { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal AveragePrice { get; set; }

    public required Direction Direction { get; set; }

    public required List<OrderDto> Orders { get; set; }
}
