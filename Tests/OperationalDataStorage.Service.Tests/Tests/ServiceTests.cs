using OperationalDataStorage.Service.Tests.Environments;
using OperationalDataStorage.Service.Tests.Factories;
using OperationalDataStorage.Service.Tests.Services;
using System.Text.Json.Serialization;
using System.Text.Json;
using OperationalDataStorage.Contracts.Ohlcs;
using FluentAssertions;
using OperationalDataStorage.Application.Services;
using System;
using OperationalDataStorage.Domain.Entities.Ohlcs;

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
    public async Task EventsSnapshotReceived_ApiReturnsLastEvent(string symbol)
    {
        // Arrange.
        await using var testEnvironment = new TestEnvironment();
        var cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ArrangeTimeoutMs);
        await testEnvironment.SetupAsync(cancellationTokenSource.Token);
        using var mediator = new OperationalDataStorageMediator(testEnvironment);
        var startInterval = IntervalDateTimeFactory.GetStartDateTime(Application.Models.Ohlcs.TimeInterval.Days1, DateTime.UtcNow);
        var endInterval = IntervalDateTimeFactory.GetEndDateTime(Application.Models.Ohlcs.TimeInterval.Days1, startInterval);
        var ohlc1 = OhlcFactory.CreateOhlc(symbol, startInterval, endInterval, Contracts.Ohlcs.TimeInterval.Days1, 10000, 100000, 1000);
        var ohlc2 = OhlcFactory.CreateOhlc(symbol, startInterval, endInterval, Contracts.Ohlcs.TimeInterval.Days1, 10000, 100000, 10001);
        var eventsSnapshot = EventsSnapshotFactory.GenerateEventsSnapshot([ohlc1, ohlc2]);

        // Act.
        mediator.Publish(eventsSnapshot, mediator.EventsSnapshotSettings.ExchangeName, mediator.EventsSnapshotSettings.RoutingKeys);
        await Task.Delay(1000); // Wait for messages to be processed.
        cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ActTimeoutMs);
        var receivedOhlc = await mediator.GetOhlcsAsync(symbol, startInterval, endInterval, Contracts.Ohlcs.TimeInterval.Days1, cancellationTokenSource.Token);
        await testEnvironment.StopAsync(CancellationToken.None);

        // Assert.
        receivedOhlc.Should().NotBeNull();
        receivedOhlc.Ohlcs.Should().NotBeNull();
        receivedOhlc.Ohlcs.Should().HaveCount(1);
        var ohlc = receivedOhlc.Ohlcs.First();

        ohlc.Should().NotBeNull();
        ohlc.Symbol.Should().Be(symbol);
        ohlc.Low.Should().Be(ohlc2.Low);
        ohlc.High.Should().Be(ohlc2.High);
        ohlc.Open.Should().Be(ohlc2.Open);
        ohlc.Close.Should().Be(ohlc2.Close);
        ohlc.StartTime.Should().Be(startInterval);
        ohlc.EndTime.Should().Be(endInterval);
        ohlc.Volume.Should().Be(ohlc2.Volume);
        //ohlcs.Should().OnlyContain(i => i.Symbol == symbol);
        //ohlcs.Should().OnlyContain(i => i.Interval == TimeInterval.Days1);

        //var dates = ohlcs.Select(i => i.StartTime.Date).Distinct().ToArray();
        //for (var i = 0; i < dates.Length; i++)
        //{
        //    var date = dates[i];
        //    var minPrice = ohlcs.Where(i => i.StartTime.Date == date).Min(i => i.Low);
        //    var maxPrice = ohlcs.Where(i => i.StartTime.Date == date).Max(i => i.High);
        //    var openPrice = ohlcs.First(i => i.StartTime.Date == date).Open;
        //    var closePrice = ohlcs.Last(i => i.StartTime.Date == date).Close;
        //    var volume = ohlcs.Where(i => i.StartTime.Date == date).Sum(i => i.Volume);

        //    var ohlc = receivedOhlc.Ohlcs.FirstOrDefault(i => i.StartTime.Date == date);
        //    ohlc.Should().NotBeNull();
        //    ohlc.Low.Should().Be(minPrice);
        //    ohlc.High.Should().Be(maxPrice);
        //    ohlc.Open.Should().Be(openPrice);
        //    ohlc.Close.Should().Be(closePrice);
        //    ohlc.EndTime.Date.Should().Be(date);
        //    ohlc.Volume.Should().Be(volume);
        //    ohlcs.Should().OnlyContain(i => i.Symbol == symbol);
        //    ohlcs.Should().OnlyContain(i => i.Interval == TimeInterval.Days1);
        //}
    }
}