namespace MarketDataAggregator.Contracts.Interests;

/// <summary>
/// Get interests request.
/// </summary>
public class GetInterestsRequest
{
    /// <summary>
    /// Strategy to filter data.
    /// </summary>
    /// <example>Turtle</example>
    public required string Strategy { get; set; }
}
