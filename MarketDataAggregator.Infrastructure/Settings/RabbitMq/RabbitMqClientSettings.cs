namespace MarketDataAggregator.Infrastructure.Settings.RabbitMq;

public class RabbitMqClientSettings
{
    public required string HostName { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
