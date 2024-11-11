using OperationalDataStorage.Application.Settings;

namespace OperationalDataStorage.Infrastructure.Settings;

public class DatabaseSettings : IValidatable
{
    public required string ConnectionString { get; set; }

    public required string DatabaseName { get; set; }

    public void Validate()
    {
        ArgumentNullException.ThrowIfNull(ConnectionString);
        ArgumentNullException.ThrowIfNull(DatabaseName);
    }
}
