namespace MarketDataAggregator.Models.Interests;

public class AddOrdersInput
{
    public required IEnumerable<OrderDto> Dtos { get; set; }
}
