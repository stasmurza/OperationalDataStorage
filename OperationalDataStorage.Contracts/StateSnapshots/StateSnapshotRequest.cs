namespace OperationalDataStorage.Contracts.StateSnapshots;

/// <summary>
/// State snapshot request.
/// </summary>
public class StateSnapshotRequest
{
    /// <summary>
    /// Strategy to filter data.
    /// </summary>
    /// <example>Turtle</example>
    public required string Strategy { get; set; }
}
