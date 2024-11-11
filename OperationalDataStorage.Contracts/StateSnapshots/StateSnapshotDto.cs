using OperationalDataStorage.Contracts.Interests;
using OperationalDataStorage.Contracts.Positions;

namespace OperationalDataStorage.Contracts.StateSnapshots;

/// <summary>
/// State snapshot DTO.
/// </summary>
public class StateSnapshotDto
{
    /// <summary>
    /// Positions.
    /// </summary>
    public required IEnumerable<PositionDto> Positions { get; set; }

    /// <summary>
    /// Interests.
    /// </summary>
    public required IEnumerable<InterestDto> Interests { get; set; }
}
