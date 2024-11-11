namespace OperationalDataStorage.Contracts.Interests;

/// <summary>
/// Interest DTO.
/// </summary>
public class InterestDto
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
    /// Average price.
    /// </summary>
    public required decimal AveragePrice { get; set; }

    /// <summary>
    /// Direction.
    /// </summary>
    public required Direction Direction { get; set; }
}
