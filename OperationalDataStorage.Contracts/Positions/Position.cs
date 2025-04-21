namespace OperationalDataStorage.Contracts.Positions;

/// <summary>
/// Position.
/// </summary>
public class Position
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
    /// Entry price.
    /// </summary>
    public required decimal EntryPrice { get; set; }

    /// <summary>
    /// Direction.
    /// </summary>
    public required Direction Direction { get; set; }

    /// <summary>
    /// Own trades.
    /// </summary>
    public required List<Guid> OrderIds { get; set; }
}
