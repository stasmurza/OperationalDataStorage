namespace OperationalDataStorage.Application.Models.Interests;

public class OrderDto
{
    public required string Id { get; set; }

    public required string Symbol { get; set; }

    public required string Strategy { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal Price { get; set; }

    public required Direction Direction { get; set; }
}
