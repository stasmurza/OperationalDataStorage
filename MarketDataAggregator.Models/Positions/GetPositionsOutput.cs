using MarketDataAggregator.Models.Aggregates.Orders;

namespace MarketDataAggregator.Models.Positions;

public class GetPositionsOutput
{
    public required IEnumerable<PositionDto> Positions { get; set; }
}
