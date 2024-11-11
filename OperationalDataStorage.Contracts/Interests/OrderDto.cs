namespace OperationalDataStorage.Contracts.Interests;

/// <summary>
/// Order DTO.
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Id.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Symbol.
    /// </summary>
    public required string Symbol { get; set; }

    /// <summary>
    /// Strategy.
    /// </summary>
    public required string Strategy { get; set; }

    /// <summary>
    /// Quantity.
    /// </summary>
    public required decimal Quantity { get; set; }

    /// <summary>
    /// Price.
    /// </summary>
    public required decimal Price { get; set; }

    /// <summary>
    /// Direction.
    /// </summary>
    public required Direction Direction { get; set; }
}
