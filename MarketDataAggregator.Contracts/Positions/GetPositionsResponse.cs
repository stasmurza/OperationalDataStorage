namespace MarketDataAggregator.Contracts.Positions;

/// <summary>
/// Positions response.
/// </summary>
public class GetPositionsResponse
{
    /// <summary>
    /// Positions.
    /// </summary>
    public required IEnumerable<PositionDto> Positions { get; set; }
}
