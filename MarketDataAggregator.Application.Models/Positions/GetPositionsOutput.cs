namespace MarketDataAggregator.Application.Models.Positions;

public class GetPositionsOutput
{
    public required IEnumerable<PositionDto> Positions { get; set; }
}
