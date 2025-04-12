namespace OperationalDataStorage.Contracts.Positions;

/// <summary>
/// Filled order DTO.
/// </summary>
public class FilledOrderDto
{
    /// <summary>
    /// Id of order.
    /// </summary>
    public required Guid OrderId { get; set; }

    /// <summary>
    /// Symbol.
    /// </summary>
    public required string Symbol { get; set; }

    /// <summary>
    /// Strategy.
    /// </summary>
    public required string Strategy { get; set; }

    /// <summary>
    /// Position direction.
    /// </summary>
    public required Direction Direction { get; set; }

    /// <summary>
    /// A quantity.
    /// </summary>
    public required decimal FilledQuantity { get; set; }

    /// <summary>
    /// Position size after execution of order.
    /// </summary>
    public required decimal FinalPositionQuantity { get; set; }

    /// <summary>
    /// An average price of all fills.
    /// </summary>
    public required decimal AverageFillPrice { get; set; }
}
