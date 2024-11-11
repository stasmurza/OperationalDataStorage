namespace OperationalDataStorage.Contracts.Interests;

/// <summary>
/// Interests response.
/// </summary>
public class GetInterestsResponse
{
    /// <summary>
    /// Interests.
    /// </summary>
    public required IEnumerable<InterestDto> Interests { get; set; }
}
