namespace OperationalDataStorage.Contracts.Positions;

/// <summary>
/// Positions response.
/// </summary>
public class GetPositionsResponse
{
    /// <summary>
    /// Positions.
    /// </summary>
    public required IEnumerable<Position> Positions { get; set; }
}
