namespace MarketDataAggregator.Contracts.Positions;

/// <summary>
/// Own trafe DTO.
/// </summary>
public class OwnTradeDto
{
    /// <summary>
    /// Id.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Date and time.
    /// </summary>
    public required DateTime DateTime { get; set; }

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
}
