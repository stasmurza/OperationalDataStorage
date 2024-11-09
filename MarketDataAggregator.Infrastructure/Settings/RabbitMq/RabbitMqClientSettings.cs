using MarketDataAggregator.Application.Settings;

namespace MarketDataAggregator.Infrastructure.Settings.RabbitMq;

public class RabbitMqClientSettings : IValidatable
{
    public required string HostName { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }

    public void Validate()
    {
        ArgumentNullException.ThrowIfNull(HostName);
        ArgumentNullException.ThrowIfNull(UserName);
        ArgumentNullException.ThrowIfNull(Password);
    }
}
