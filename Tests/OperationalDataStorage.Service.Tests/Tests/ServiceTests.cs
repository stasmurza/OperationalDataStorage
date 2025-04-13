using OperationalDataStorage.Contracts.Positions;
using OperationalDataStorage.Service.Tests.Environments;
using OperationalDataStorage.Service.Tests.Factories;
using OperationalDataStorage.Service.Tests.Services;
using System.Text.Json.Serialization;
using System.Text.Json;
using OperationalDataStorage.Contracts.Ohlcs;

namespace OperationalDataStorage.Service.Tests.Tests;

public class ServiceTests
{
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public Tests()
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
        using var mediator = new EventStoreMediator(testEnvironment);
        var endInterval = DateTime.UtcNow;
        var startInterval = endInterval.Date.AddDays(-5).Date;
        var ohlcs = OhlcFactory.CreateOhlcs(symbol, startInterval, endInterval, TimeInterval.Minutes1, 10000, 100000).ToArray();
        var eventsSnapshot = EventsSnapshotFactory.GenerateEventsSnapshot(ohlcs);

        // Act.
        mediator.Publish(eventsSnapshot, mediator.EventsSnapshotSettings.ExchangeName, mediator.EventsSnapshotSettings.RoutingKeys);
        cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ActTimeoutMs);
        await testEnvironment.StopAsync(CancellationToken.None);

        // Assert.
        var response = await mediator.GetOhlcsAsync(symbol, startInterval, endInterval, TimeInterval.Days1, cancellationTokenSource.Token);
        var dates = ohlcs.Select(i => i.StartTime.Date).Distinct().ToArray();
        for (var i = 0; i < dates.Length; i++)
        {
            var date = dates[i];
            var minPrice = ohlcs.Where(i => i.StartTime.Date == date).Min(i => i.Low);
            var maxPrice = ohlcs.Where(i => i.StartTime.Date == date).Max(i => i.High);
            var openPrice = ohlcs.First(i => i.StartTime.Date == date).Open;
            var closePrice = ohlcs.Last(i => i.StartTime.Date == date).Close;

            var ohlc = response.Ohlcs.FirstOrDefault(i => i.StartTime.Date == date);
            ohlc.Low.Should().NotBeNull();
            ohlcEvent!.EventDateTime.Should().Be(ohlc.EventDateTime);
            ohlcEvent!.EntityId.Should().Be(ohlc.EventId.ToString());
            ohlcEvent!.EntityType.Should().Be(Contracts.Events.EntityType.Ohlc);
            var receivedOhlc = JsonSerializer.Deserialize<Ohlc>(ohlcEvent.EventData, jsonSerializerOptions);
            receivedOhlc.Should().NotBeNull();
            receivedOhlc.Should().BeEquivalentTo(ohlc);
        }
    }

    [Test]
    [TestCase("PI_XBTUSD")]
    public async Task OhlcMinutesEventsReceived_EventsSnapshotShouldHasCorrectEvents(string symbol)
    {
        // Arrange.
        await using var testEnvironment = new TestEnvironment();
        var cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ArrangeTimeoutMs);
        await testEnvironment.SetupAsync(cancellationTokenSource.Token);
        using var mediator = new EventStoreMediator(testEnvironment);
        var dateTime = DateTime.UtcNow;
        var ohlcs = OhlcFactory.CreateOhlcs(symbol, dateTime.AddMinutes(-10), dateTime, TimeInterval.Minutes1).ToArray();

        // Act.
        foreach (var ohlc in ohlcs)
        {
            mediator.Publish(ohlc, mediator.MarketDataSettings.ExchangeName, mediator.MarketDataSettings.RoutingKeys);
        }

        cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ActTimeoutMs);
        var eventsSnapshot = await mediator.ReadFirstEventsSnapshotAsync(cancellationTokenSource.Token);
        await testEnvironment.StopAsync(CancellationToken.None);

        // Assert.
        for (var i = 0; i < ohlcs.Length; i++)
        {
            var ohlc = ohlcs[i];
            var ohlcEvent = eventsSnapshot.NewEvents.FirstOrDefault(i => i.EntityId == ohlc.EventId.ToString());
            ohlcEvent.Should().NotBeNull();
            ohlcEvent!.EventDateTime.Should().Be(ohlc.EventDateTime);
            ohlcEvent!.EntityId.Should().Be(ohlc.EventId.ToString());
            ohlcEvent!.EntityType.Should().Be(Contracts.Events.EntityType.Ohlc);
            var receivedOhlc = JsonSerializer.Deserialize<Ohlc>(ohlcEvent.EventData, jsonSerializerOptions);
            receivedOhlc.Should().NotBeNull();
            receivedOhlc.Should().BeEquivalentTo(ohlc);
        }
    }

    [Test]
    [TestCase("PI_XBTUSD", "Turtles")]
    public async Task LongTradeRequestEventReceived_EventsSnapshotShouldHasCorrectEvents(string symbol, string strategy)
    {
        // Arrange.
        await using var testEnvironment = new TestEnvironment();
        var cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ArrangeTimeoutMs);
        await testEnvironment.SetupAsync(cancellationTokenSource.Token);
        using var mediator = new EventStoreMediator(testEnvironment);
        var longTradeRequest = TradeRequestFactory.GenerateLongTradeRequest(symbol, strategy);

        // Act.
        mediator.Publish(longTradeRequest, mediator.TradeRequestSettings.ExchangeName, mediator.TradeRequestSettings.RoutingKeys);

        cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ActTimeoutMs);
        var eventsSnapshot = await mediator.ReadFirstEventsSnapshotAsync(cancellationTokenSource.Token);
        await testEnvironment.StopAsync(CancellationToken.None);

        // Assert.
        var tradeRequestEvent = eventsSnapshot.NewEvents.FirstOrDefault(i => i.EventId == longTradeRequest.EventId);
        tradeRequestEvent.Should().NotBeNull();
        tradeRequestEvent!.EventDateTime.Should().Be(longTradeRequest.EventDateTime);
        tradeRequestEvent!.EntityId.Should().Be(longTradeRequest.Id.ToString());
        tradeRequestEvent!.EntityType.Should().Be(Contracts.Events.EntityType.TradeRequest);
        var receivedTradeRequest = JsonSerializer.Deserialize<TradeRequest>(tradeRequestEvent.EventData, jsonSerializerOptions);
        receivedTradeRequest.Should().NotBeNull();
        receivedTradeRequest.Should().BeEquivalentTo(longTradeRequest);
    }

    [Test]
    [TestCase("PI_XBTUSD", "Turtles")]
    public async Task ShortTradeRequestEventReceived_EventsSnapshotShouldHasCorrectEvents(string symbol, string strategy)
    {
        // Arrange.
        await using var testEnvironment = new TestEnvironment();
        var cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ArrangeTimeoutMs);
        await testEnvironment.SetupAsync(cancellationTokenSource.Token);
        using var mediator = new EventStoreMediator(testEnvironment);
        var shortTradeRequest = TradeRequestFactory.GenerateLongTradeRequest(symbol, strategy);

        // Act.
        mediator.Publish(shortTradeRequest, mediator.TradeRequestSettings.ExchangeName, mediator.TradeRequestSettings.RoutingKeys);

        cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ActTimeoutMs);
        var eventsSnapshot = await mediator.ReadFirstEventsSnapshotAsync(cancellationTokenSource.Token);
        await testEnvironment.StopAsync(CancellationToken.None);

        // Assert.
        var tradeRequestEvent = eventsSnapshot.NewEvents.FirstOrDefault(i => i.EventId == shortTradeRequest.EventId);
        tradeRequestEvent.Should().NotBeNull();
        tradeRequestEvent!.EventDateTime.Should().Be(shortTradeRequest.EventDateTime);
        tradeRequestEvent!.EntityId.Should().Be(shortTradeRequest.Id.ToString());
        tradeRequestEvent!.EntityType.Should().Be(Contracts.Events.EntityType.TradeRequest);
        var receivedTradeRequest = JsonSerializer.Deserialize<TradeRequest>(tradeRequestEvent.EventData, jsonSerializerOptions);
        receivedTradeRequest.Should().NotBeNull();
        receivedTradeRequest.Should().BeEquivalentTo(shortTradeRequest);
    }

    [Test]
    [TestCase("PI_XBTUSD", "Turtles")]
    public async Task FilledOrderEventReceived_EventsSnapshotShouldHasCorrectEvents(string symbol, string strategy)
    {
        // Arrange.
        await using var testEnvironment = new TestEnvironment();
        var cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ArrangeTimeoutMs);
        await testEnvironment.SetupAsync(cancellationTokenSource.Token);
        using var mediator = new EventStoreMediator(testEnvironment);
        var longTradeRequest = TradeRequestFactory.GenerateLongTradeRequest(symbol, strategy);
        var filledOrder = FilledOrderFactory.GenerateFilledOrder(longTradeRequest);

        // Act.
        mediator.Publish(filledOrder, mediator.FilledOrderSettings.ExchangeName, mediator.FilledOrderSettings.RoutingKeys);

        cancellationTokenSource = new CancellationTokenSource(testEnvironment.TestsSettings.ActTimeoutMs);
        var eventsSnapshot = await mediator.ReadFirstEventsSnapshotAsync(cancellationTokenSource.Token);
        await testEnvironment.StopAsync(CancellationToken.None);

        // Assert.
        var filledOrderEvent = eventsSnapshot.NewEvents.FirstOrDefault(i => i.EventId == filledOrder.EventId);
        filledOrderEvent.Should().NotBeNull();
        filledOrderEvent!.EventDateTime.Should().Be(filledOrder.EventDateTime);
        filledOrderEvent!.EntityId.Should().Be(filledOrder.OrderId.ToString());
        filledOrderEvent!.EntityType.Should().Be(Contracts.Events.EntityType.FilledOrder);
        var receivedFilledOrder = JsonSerializer.Deserialize<FilledOrder>(filledOrderEvent.EventData, jsonSerializerOptions);
        receivedFilledOrder.Should().NotBeNull();
        receivedFilledOrder.Should().BeEquivalentTo(filledOrder);
    }
}