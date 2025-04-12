using OperationalDataStorage.Application.Settings;

namespace OperationalDataStorage.Infrastructure.Models.Settings;

public class DatabaseSettings : IValidatable
{
    public required string ConnectionString { get; set; }

    public required string HostName { get; set; }

    public required string DatabaseName { get; set; }

    public required int Port { get; set; }

    public void Validate()
    {
        ArgumentException.ThrowIfNullOrEmpty(ConnectionString);
        ArgumentNullException.ThrowIfNull(HostName);
        ArgumentNullException.ThrowIfNull(DatabaseName);

        if (Port == 0) throw new InvalidOperationException($"{nameof(Port)} is empty.");
    }
}
