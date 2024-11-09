using MarketDataAggregator.Infrastructure;

namespace MarketDataAggregator.Presentation.HostedServices;

public sealed class MessageConsumersHostedService(
    EventsSnapshotConsumer eventsSnapshotConsumer,
    StateSnapshotConsumer stateSnapshotConsumer) : IHostedService, IDisposable
{
    private readonly EventsSnapshotConsumer eventsSnapshotConsumer = eventsSnapshotConsumer;
    private readonly StateSnapshotConsumer stateSnapshotConsumer = stateSnapshotConsumer;
    private bool disposedValue;

    public Task StartAsync(CancellationToken cancellationToken) =>Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects)
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer
            // Set large fields to null
            disposedValue = true;
            eventsSnapshotConsumer.Dispose();
            stateSnapshotConsumer.Dispose();

        }
    }

    // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources.
    ~MessageConsumersHostedService()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method.
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method.
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
