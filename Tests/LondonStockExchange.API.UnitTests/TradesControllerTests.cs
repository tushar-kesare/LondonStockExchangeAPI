using FluentAssertions;
using LondonStockExchange.API.Models;
using LondonStockExchange.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LondonStockExchange.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;
using Stock = LondonStockExchange.Domain.Entities.Stock;

namespace LondonStockExchange.API.UnitTests;

public class TradesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly TestRepository _testRepository = new TestRepository();
    private readonly Mock<IDateTimeProvider> _dateTimeProvider = new Mock<IDateTimeProvider>();

    public TradesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient()
    {
        return _factory.WithWebHostBuilder(
                builder => builder.ConfigureTestServices(
                    services =>
                    {
                        ServiceCollectionServiceExtensions.AddScoped<IRepository>(services, _ => _testRepository);
                    }))
            .CreateClient();
    }

    private HttpClient CreateClient(IRepository repository, IDateTimeProvider dateTimeProvider)
    {
        return _factory.WithWebHostBuilder(
                builder => builder.ConfigureTestServices(
                    services =>
                    {
                        ServiceCollectionServiceExtensions.AddScoped<IRepository>(services, _ => repository);
                        ServiceCollectionServiceExtensions.AddScoped<IDateTimeProvider>(services, _ => dateTimeProvider);
                    }))
            .CreateClient();
    }

    [Theory]
    [InlineData(null, 11, 10, 1)]
    [InlineData("APPL", -1, 10, 1)]
    [InlineData("APPL", 11, 0, 1)]
    [InlineData("APPL", 11, 10, 0)]
    public async Task ReceiveTradeNotification_WhenInvalidData_ShouldReturnBadRequest(string tickerSymbol,
        decimal price, double shares, int brokerId)
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response =
            await client.PostAsJsonAsync<ReceiveTradeNotificationRequest>("/api/trades",
                new ReceiveTradeNotificationRequest
                {
                    TickerSymbol = tickerSymbol,
                    Price = price,
                    Shares = shares,
                    BrokerId = brokerId
                });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReceiveTradeNotification_WhenStockNotFound_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        _testRepository.Stocks.Add(new Stock { Id = 1, TickerSymbol = "MSFT", Price = 10 });

        var client = CreateClient();

        // Act
        var response =
            await client.PostAsJsonAsync<ReceiveTradeNotificationRequest>("/api/trades",
                new ReceiveTradeNotificationRequest
                {
                    TickerSymbol = "APPL",
                    Price = 12.4m,
                    Shares = 13,
                    BrokerId = 6478
                });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task ReceiveTradeNotification_WhenValidData_ShouldStoreTrade()
    {
        // Arrange
        var stockId = 1;
        var tickerSymbol = "Appl";
        var price = 12.4m;
        var shares = 13.4;
        var brokerId = 6487;
        var utcNow = DateTime.UtcNow;

        _testRepository.Stocks.Add(new Stock { Id = stockId, TickerSymbol = tickerSymbol.ToUpper(), Price = 10 });
        _dateTimeProvider.Setup(x => x.UtcNow).Returns(utcNow);

        var client = CreateClient(_testRepository, _dateTimeProvider.Object);

        // Act
        var response =
            await client.PostAsJsonAsync<ReceiveTradeNotificationRequest>("/api/trades",
                new ReceiveTradeNotificationRequest
                {
                    TickerSymbol = tickerSymbol,
                    Price = price,
                    Shares = shares,
                    BrokerId = brokerId
                });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        _testRepository.Trades.Any(t =>
                t.StockId == stockId
                && t.Price == price
                && t.Shares == shares
                && t.BrokerId == brokerId
                && t.Timestamp == utcNow)
            .Should().BeTrue();
    }

    [Fact]
    public async Task ReceiveTradeNotification_WhenErrorProcessingRequest_ShouldReturnInternalServerError()
    {
        // Arrange
        var repository = new Mock<IRepository>();
        repository.Setup(r => r.GetStockByTickerSymbol(It.IsAny<string>())).ThrowsAsync(new Exception());

        var client = CreateClient(repository.Object, _dateTimeProvider.Object);

        // Act
        var response =
            await client.PostAsJsonAsync<ReceiveTradeNotificationRequest>("/api/trades",
                new ReceiveTradeNotificationRequest
                {
                    TickerSymbol = "Appl",
                    Price = 184.56m,
                    Shares = 10.4,
                    BrokerId = 574754
                });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var responseText = await response.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<ProblemDetails>(responseText);
        responseObj.Detail.Should().Be("Error occurred while processing request");
    }
}