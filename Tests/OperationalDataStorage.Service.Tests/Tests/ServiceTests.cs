using OperationalDataStorage.Service.Tests.Environments;
using OperationalDataStorage.Service.Tests.Factories;
using OperationalDataStorage.Service.Tests.Services;
using System.Text.Json.Serialization;
using System.Text.Json;
using OperationalDataStorage.Contracts.Ohlcs;
using FluentAssertions;

namespace OperationalDataStorage.Service.Tests.Tests;

public class ServiceTests
{
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public ServiceTests()
    {
        jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    [Test]
    [TestCase("PI_XBTUSD")]
    public async Task OhlcDaysEventsReceived_EventsSnapshotShouldHasCorrectEvents(string symbol)
    {
        // Arrange.
        await using var testEnvironment = new TestEnvironment();
        var cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ArrangeTimeoutMs);
        await testEnvironment.SetupAsync(cancellationTokenSource.Token);
        using var mediator = new OperationalDataStorageMediator(testEnvironment);
        var endInterval = DateTime.UtcNow;
        var startInterval = endInterval.Date.AddDays(-5).Date;
        var ohlcs = OhlcFactory.CreateOhlcs(symbol, startInterval, endInterval, TimeInterval.Minutes1, 10000, 100000).ToArray();
        var eventsSnapshot = EventsSnapshotFactory.GenerateEventsSnapshot(ohlcs);

        // Act.
        mediator.Publish(eventsSnapshot, mediator.EventsSnapshotSettings.ExchangeName, mediator.EventsSnapshotSettings.RoutingKeys);
        cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ActTimeoutMs);
        await testEnvironment.StopAsync(CancellationToken.None);

        // Assert.
        var receivedOhlc = await mediator.GetOhlcsAsync(symbol, startInterval, endInterval, TimeInterval.Days1, cancellationTokenSource.Token);
        var dates = ohlcs.Select(i => i.StartTime.Date).Distinct().ToArray();
        for (var i = 0; i < dates.Length; i++)
        {
            var date = dates[i];
            var minPrice = ohlcs.Where(i => i.StartTime.Date == date).Min(i => i.Low);
            var maxPrice = ohlcs.Where(i => i.StartTime.Date == date).Max(i => i.High);
            var openPrice = ohlcs.First(i => i.StartTime.Date == date).Open;
            var closePrice = ohlcs.Last(i => i.StartTime.Date == date).Close;
            var volume = ohlcs.Where(i => i.StartTime.Date == date).Sum(i => i.Volume);

            var ohlc = receivedOhlc.Ohlcs.FirstOrDefault(i => i.StartTime.Date == date);
            ohlc.Should().NotBeNull();
            ohlc.Low.Should().Be(minPrice);
            ohlc.High.Should().Be(maxPrice);
            ohlc.Open.Should().Be(openPrice);
            ohlc.Close.Should().Be(closePrice);
            ohlc.EndTime.Date.Should().Be(date);
            ohlc.Volume.Should().Be(volume);
            ohlcs.Should().OnlyContain(i => i.Symbol == symbol);
            ohlcs.Should().OnlyContain(i => i.Interval == TimeInterval.Days1);
        }
    }
}