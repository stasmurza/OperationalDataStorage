namespace MarketDataAggregator.Contracts.Positions;

public class GetPositionsResponse
{
    public required IEnumerable<PositionDto> Positions { get; set; }
}
