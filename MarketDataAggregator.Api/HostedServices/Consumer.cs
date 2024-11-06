using EventStore.Infrastructure;

namespace MarketDataAggregator.Api.HostedServices;

public sealed class Consumer : IHostedService, IDisposable
{
    private readonly RabbitMqConsumer rabbitMqConsumer;
    private bool disposedValue;

    public Consumer(RabbitMqConsumer rabbitMqConsumer)
    {
        this.rabbitMqConsumer = rabbitMqConsumer;
    }

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
            rabbitMqConsumer.Dispose();
        }
    }

    // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources.
    ~Consumer()
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
