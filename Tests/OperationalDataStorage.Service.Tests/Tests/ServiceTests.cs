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
using OperationalDataStorage.Contracts.Positions;

namespace OperationalDataStorage.Service.Tests.Tests;

public class ServiceTests
{
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public ServiceTests()
    {
        jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    /// <summary>
    /// Ohlcs are out of order. The message with biggest volume is returned.
    /// </summary>
    [Test]
    [TestCase("PI_XBTUSD")]
    public async Task OhlcsReceived_ApiReturnsBiggestVolumeOhlc(string symbol)
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
        var ohlc3 = OhlcFactory.CreateOhlc(symbol, startInterval, endInterval, Contracts.Ohlcs.TimeInterval.Days1, 10000, 100000, 1000);
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
    }

    /// <summary>
    /// New position received. API returns correct position.
    /// </summary>
    [Test]
    [TestCase("PI_XBTUSD", "test", Contracts.Direction.Buy)]
    [TestCase("PI_XBTUSD", "test", Contracts.Direction.Sell)]
    public async Task NewPositionReceived_ApiReturnsCorrectPosition(string symbol, string strategy, Contracts.Direction direction)
    {
        // Arrange.
        await using var testEnvironment = new TestEnvironment();
        var cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ArrangeTimeoutMs);
        await testEnvironment.SetupAsync(cancellationTokenSource.Token);
        using var mediator = new OperationalDataStorageMediator(testEnvironment);
        var filledOrder1 = FilledOrderFactory.GenerateFilledOrder(symbol, strategy, direction, 100, 1000);
        var filledOrder2 = FilledOrderFactory.GenerateFilledOrder(symbol, strategy, direction, 100, 1000);
        var eventsSnapshot = EventsSnapshotFactory.GenerateEventsSnapshot([filledOrder1, filledOrder2]);

        // Act.
        mediator.Publish(eventsSnapshot, mediator.EventsSnapshotSettings.ExchangeName, mediator.EventsSnapshotSettings.RoutingKeys);
        await Task.Delay(1000); // Wait for messages to be processed.
        cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ActTimeoutMs);
        var positions = await mediator.GetPositionsAsync(strategy, cancellationTokenSource.Token);
        await testEnvironment.StopAsync(CancellationToken.None);

        // Assert.
        positions.Should().NotBeNull();
        positions.Positions.Should().NotBeNull();
        positions.Positions.Should().HaveCount(1);
        var position = positions.Positions.First();

        var amount = filledOrder1.AverageFillPrice * filledOrder1.FilledQuantity + filledOrder2.AverageFillPrice * filledOrder2.FilledQuantity;
        var averageFillPrice = amount / (filledOrder1.FilledQuantity + filledOrder2.FilledQuantity);
        position.Should().NotBeNull();
        position.Symbol.Should().Be(symbol);
        position.Strategy.Should().Be(strategy);
        position.Quantity.Should().Be(filledOrder1.FilledQuantity + filledOrder2.FilledQuantity);
        position.EntryPrice.Should().Be(averageFillPrice);
        position.Direction.Should().Be(direction);
        position.OrderIds.Should().Contain(filledOrder1.OrderId);
        position.OrderIds.Should().Contain(filledOrder2.OrderId);
    }

    /// <summary>
    /// Existing position decreased. API returns correct position.
    /// </summary>
    [Test]
    [TestCase("PI_XBTUSD", "test", Contracts.Direction.Buy)]
    [TestCase("PI_XBTUSD", "test", Contracts.Direction.Sell)]
    public async Task ExistingPositionDecreased_ApiReturnsCorrectPosition(string symbol, string strategy, Contracts.Direction direction)
    {
        // Arrange.
        await using var testEnvironment = new TestEnvironment();
        var cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ArrangeTimeoutMs);
        await testEnvironment.SetupAsync(cancellationTokenSource.Token);
        using var mediator = new OperationalDataStorageMediator(testEnvironment);
        var oppositeDirection = direction == Contracts.Direction.Buy ? Contracts.Direction.Sell : Contracts.Direction.Buy;
        var filledOrder1 = FilledOrderFactory.GenerateFilledOrder(symbol, strategy, direction, 100, 1000);
        var filledOrder2 = FilledOrderFactory.GenerateFilledOrder(symbol, strategy, oppositeDirection, 75, 1000);
        var eventsSnapshot1 = EventsSnapshotFactory.GenerateEventsSnapshot([filledOrder1]);
        var eventsSnapshot2 = EventsSnapshotFactory.GenerateEventsSnapshot([filledOrder2]);

        // Act.
        mediator.Publish(eventsSnapshot1, mediator.EventsSnapshotSettings.ExchangeName, mediator.EventsSnapshotSettings.RoutingKeys);
        mediator.Publish(eventsSnapshot2, mediator.EventsSnapshotSettings.ExchangeName, mediator.EventsSnapshotSettings.RoutingKeys);
        await Task.Delay(1000); // Wait for messages to be processed.
        cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ActTimeoutMs);
        var positions = await mediator.GetPositionsAsync(strategy, cancellationTokenSource.Token);
        await testEnvironment.StopAsync(CancellationToken.None);

        // Assert.
        positions.Should().NotBeNull();
        positions.Positions.Should().NotBeNull();
        positions.Positions.Should().HaveCount(1);
        var position = positions.Positions.First();

        var amount = filledOrder1.AverageFillPrice * filledOrder1.FilledQuantity - filledOrder2.AverageFillPrice * filledOrder2.FilledQuantity;
        var averageFillPrice = amount / (filledOrder1.FilledQuantity - filledOrder2.FilledQuantity);
        position.Should().NotBeNull();
        position.Symbol.Should().Be(symbol);
        position.Strategy.Should().Be(strategy);
        position.Quantity.Should().Be(filledOrder1.FilledQuantity - filledOrder2.FilledQuantity);
        position.EntryPrice.Should().Be(averageFillPrice);
        position.Direction.Should().Be(direction);
        position.OrderIds.Should().Contain(filledOrder1.OrderId);
        position.OrderIds.Should().Contain(filledOrder2.OrderId);
    }

    /// <summary>
    /// Existing position closed. API returns correct position.
    /// </summary>
    [Test]
    [TestCase("PI_XBTUSD", "test", Contracts.Direction.Buy)]
    [TestCase("PI_XBTUSD", "test", Contracts.Direction.Sell)]
    public async Task ExistingPositionClosed_ApiReturnsCorrectPosition(string symbol, string strategy, Contracts.Direction direction)
    {
        // Arrange.
        await using var testEnvironment = new TestEnvironment();
        var cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ArrangeTimeoutMs);
        await testEnvironment.SetupAsync(cancellationTokenSource.Token);
        using var mediator = new OperationalDataStorageMediator(testEnvironment);
        var oppositeDirection = direction == Contracts.Direction.Buy ? Contracts.Direction.Sell : Contracts.Direction.Buy;
        var filledOrder1 = FilledOrderFactory.GenerateFilledOrder(symbol, strategy, direction, 100, 1000);
        var filledOrder2 = FilledOrderFactory.GenerateFilledOrder(symbol, strategy, oppositeDirection, 100, 1000);
        var eventsSnapshot1 = EventsSnapshotFactory.GenerateEventsSnapshot([filledOrder1]);
        var eventsSnapshot2 = EventsSnapshotFactory.GenerateEventsSnapshot([filledOrder2]);

        // Act.
        mediator.Publish(eventsSnapshot1, mediator.EventsSnapshotSettings.ExchangeName, mediator.EventsSnapshotSettings.RoutingKeys);
        mediator.Publish(eventsSnapshot2, mediator.EventsSnapshotSettings.ExchangeName, mediator.EventsSnapshotSettings.RoutingKeys);
        await Task.Delay(1000); // Wait for messages to be processed.
        cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ActTimeoutMs);
        var positions = await mediator.GetPositionsAsync(strategy, cancellationTokenSource.Token);
        await testEnvironment.StopAsync(CancellationToken.None);

        // Assert.
        positions.Should().NotBeNull();
        positions.Positions.Should().NotBeNull();
        positions.Positions.Should().HaveCount(1);
        var position = positions.Positions.First();

        var amount = filledOrder1.AverageFillPrice * filledOrder1.FilledQuantity - filledOrder2.AverageFillPrice * filledOrder2.FilledQuantity;
        var averageFillPrice = amount / (filledOrder1.FilledQuantity - filledOrder2.FilledQuantity);
        position.Should().NotBeNull();
        position.Symbol.Should().Be(symbol);
        position.Strategy.Should().Be(strategy);
        position.Quantity.Should().Be(filledOrder1.FilledQuantity - filledOrder2.FilledQuantity);
        position.EntryPrice.Should().Be(averageFillPrice);
        position.Direction.Should().Be(direction);
        position.OrderIds.Should().Contain(filledOrder1.OrderId);
        position.OrderIds.Should().Contain(filledOrder2.OrderId);
    }

    /// <summary>
    /// Existing position closed. API returns correct position.
    /// </summary>
    [Test]
    [TestCase("PI_XBTUSD", "test", Contracts.Direction.Buy)]
    [TestCase("PI_XBTUSD", "test", Contracts.Direction.Sell)]
    public async Task OrderRedelivered_ApiReturnsCorrectPosition(string symbol, string strategy, Contracts.Direction direction)
    {
        // Arrange.
        await using var testEnvironment = new TestEnvironment();
        var cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ArrangeTimeoutMs);
        await testEnvironment.SetupAsync(cancellationTokenSource.Token);
        using var mediator = new OperationalDataStorageMediator(testEnvironment);
        var oppositeDirection = direction == Contracts.Direction.Buy ? Contracts.Direction.Sell : Contracts.Direction.Buy;
        var filledOrder = FilledOrderFactory.GenerateFilledOrder(symbol, strategy, direction, 100, 1000);
        var eventsSnapshot = EventsSnapshotFactory.GenerateEventsSnapshot([filledOrder]);

        // Act.
        mediator.Publish(eventsSnapshot, mediator.EventsSnapshotSettings.ExchangeName, mediator.EventsSnapshotSettings.RoutingKeys);
        mediator.Publish(eventsSnapshot, mediator.EventsSnapshotSettings.ExchangeName, mediator.EventsSnapshotSettings.RoutingKeys);
        await Task.Delay(1000); // Wait for messages to be processed.
        cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ActTimeoutMs);
        var positions = await mediator.GetPositionsAsync(strategy, cancellationTokenSource.Token);
        await testEnvironment.StopAsync(CancellationToken.None);

        // Assert.
        positions.Should().NotBeNull();
        positions.Positions.Should().NotBeNull();
        positions.Positions.Should().HaveCount(1);
        var position = positions.Positions.First();

        var amount = filledOrder.AverageFillPrice * filledOrder.FilledQuantity;
        var averageFillPrice = amount / filledOrder.FilledQuantity;
        position.Should().NotBeNull();
        position.Symbol.Should().Be(symbol);
        position.Strategy.Should().Be(strategy);
        position.Quantity.Should().Be(filledOrder.FilledQuantity);
        position.EntryPrice.Should().Be(averageFillPrice);
        position.Direction.Should().Be(direction);
        position.OrderIds.Should().HaveCount(1);
        position.OrderIds.Should().Contain(filledOrder.OrderId);
    }
}