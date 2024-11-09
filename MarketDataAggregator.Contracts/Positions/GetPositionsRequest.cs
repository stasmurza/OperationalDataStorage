namespace MarketDataAggregator.Contracts.Positions;

/// <summary>
/// Get positions request.
/// </summary>
public class GetPositionsRequest
{
    /// <summary>
    /// Strategy.
    /// </summary>
    /// <example>Turtle</example>
    public required string Strategy { get; set; }
}
