using OperationalDataStorage.Application.Settings;

namespace OperationalDataStorage.Infrastructure.Models.Settings.RabbitMq;

public class RabbitMqClientSettings : IValidatable
{
    public required string HostName { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required int Port { get; set; }
    public required int ManagementPort { get; set; }

    public void Validate()
    {
        ArgumentException.ThrowIfNullOrEmpty(HostName);
        ArgumentException.ThrowIfNullOrEmpty(UserName);
        ArgumentException.ThrowIfNullOrEmpty(Password);

        if (Port <= 0) throw new ArgumentException(nameof(Port));
        if (ManagementPort <= 0) throw new ArgumentException(nameof(ManagementPort));
    }
}
