namespace MarketDataAggregator.Infrastructure.Settings;

public class DatabaseSettings
{
    public required string ConnectionString { get; set; }

    public required string DatabaseName { get; set; }
}
