using FluentAssertions;
using LondonStockExchange.API.Models;
using LondonStockExchange.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Stock = LondonStockExchange.Domain.Entities.Stock;

namespace LondonStockExchange.API.UnitTests
{
    public class StocksControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly TestRepository _testRepository = new TestRepository();

        public StocksControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private HttpClient CreateClient()
        {
            return _factory
                .WithWebHostBuilder(
                    builder => builder.ConfigureTestServices(
                        services => ServiceCollectionServiceExtensions.AddScoped<IRepository>(services, _ => _testRepository)))
                .CreateClient();
        }

        [Fact]
        public async Task GetStock_WhenStockFound_ShouldReturnStockDetails()
        {
            // Arrange
            _testRepository.Stocks.Add(new Stock() { Id = 1, TickerSymbol = "APPL", Price = 180 });

            var client = CreateClient();

            // Act
            var response = await client.GetAsync("/api/stocks/appl");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseText = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<GetStockResponse>(responseText);

            responseObj.Should().Be(new GetStockResponse(new Models.Stock("APPL", 180)));
        }

        [Fact]
        public async Task GetStock_WhenStockNotFound_ReturnsNotFound()
        {
            // Arrange
            _testRepository.Stocks.Add(new Stock() { Id = 1, TickerSymbol = "APPL", Price = 180 });

            var client = CreateClient();

            // Act
            var response = await client.GetAsync("/api/stocks/msft");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetStocks_WhenCalledWithoutQueryParam_ShouldReturnAllStocks()
        {
            // Arrange
            _testRepository.Stocks.Add(new Stock() { Id = 1, TickerSymbol = "APPL", Price = 180 });
            _testRepository.Stocks.Add(new Stock() { Id = 2, TickerSymbol = "MSFT", Price = 328.2M });

            var client = CreateClient();

            // Act
            var response = await client.GetAsync("/api/stocks/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseText = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<GetStocksResponse>(responseText);

            responseObj.Stocks.Count.Should().Be(_testRepository.Stocks.Count);

            foreach (var stock in _testRepository.Stocks)
            {
                responseObj.Stocks.Any(s => s.TickerSymbol == stock.TickerSymbol && s.Price == stock.Price).Should()
                    .BeTrue();
            }

        }

        [Fact]
        public async Task GetStocks_WhenCalledWithQueryParam_ShouldReturnRequestedStocks()
        {
            // Arrange
            _testRepository.Stocks.Add(new Stock() { Id = 1, TickerSymbol = "APPL", Price = 180 });
            _testRepository.Stocks.Add(new Stock() { Id = 2, TickerSymbol = "MSFT", Price = 328.7M });
            _testRepository.Stocks.Add(new Stock() { Id = 3, TickerSymbol = "GOOGL", Price = 141.2M });
            _testRepository.Stocks.Add(new Stock() { Id = 4, TickerSymbol = "NVDA", Price = 479.8M });

            var client = CreateClient();

            // Act
            var response = await client.GetAsync("/api/stocks/?tickerSymbols=msft,nvda");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseText = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<GetStocksResponse>(responseText);

            responseObj.Stocks.Count.Should().Be(2);
            responseObj.Stocks.Any(s =>
                s.TickerSymbol == "NVDA" &&
                s.Price == _testRepository.Stocks.First(_ => _.TickerSymbol == "NVDA").Price).Should().BeTrue();
            responseObj.Stocks.Any(s =>
                s.TickerSymbol == "MSFT" &&
                s.Price == _testRepository.Stocks.First(_ => _.TickerSymbol == "MSFT").Price).Should().BeTrue();
        }
    }
}