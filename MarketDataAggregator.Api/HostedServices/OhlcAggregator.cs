using MarketDataAggregator.Models;
using MediatR;
using System.Timers;

namespace MarketDataAggregator.Api.HostedServices;

public class OhlcAggregator : IHostedService, IDisposable
{
    private readonly IMediator mediator;
    private readonly System.Timers.Timer timer = new();
    private bool disposedValue;

    public OhlcAggregator(IMediator mediator)
    {
        this.mediator = mediator;
        timer.Elapsed += Timer_Elapsed;
        timer.Interval = 5000;
        timer.Enabled = true;
    }

    private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        await mediator.Send(new AggregateOhlcsInput());
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        timer.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer.Stop();

        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
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
            timer.Dispose();
        }
    }

    // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources.
    ~OhlcAggregator()
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
