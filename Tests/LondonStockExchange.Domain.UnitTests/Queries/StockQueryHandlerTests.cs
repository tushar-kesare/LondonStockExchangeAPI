using FluentAssertions;
using LondonStockExchange.Domain.Entities;
using LondonStockExchange.Domain.Interfaces;
using LondonStockExchange.Domain.Queries;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LondonStockExchange.Domain.UnitTests.Queries
{
    public class StockQueryHandlerTests
    {
        private readonly Mock<IRepository> _repository = new Mock<IRepository>();


        private StockQueryHandler CreateQueryHandler()
        {
            return new StockQueryHandler(_repository.Object);
        }

        [Fact]
        public async Task HandleGetStockQuery_IfStockExists_ShouldReturnStock()
        {
            // Arrange
            var stock = new Stock() { Id = 1, TickerSymbol = "APPL", Price = 184 };

            _repository.Setup(r => r.GetStockByTickerSymbol(stock.TickerSymbol)).ReturnsAsync(stock);

            var sut = CreateQueryHandler();

            // Act
            var result = await sut.Handle(new GetStockQuery(stock.TickerSymbol), CancellationToken.None);

            // Assert
            result.Should().Be(stock);
        }

        [Fact]
        public async Task HandleGetStockQuery_IfStockDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var stock = new Stock() { Id = 1, TickerSymbol = "APPL", Price = 184.5m };

            _repository.Setup(r => r.GetStockByTickerSymbol(stock.TickerSymbol)).ReturnsAsync((Stock)null!);

            var sut = CreateQueryHandler();

            // Act
            var result = await sut.Handle(new GetStockQuery(stock.TickerSymbol), CancellationToken.None);

            // Assert
            result.Should().Be(null);
        }

        [Fact]
        public async Task HandleGetStocksQuery_IfNoTickerSymbolsProvided_ShouldReturnAllStocks()
        {
            // Arrange
            var stock1 = new Stock() { Id = 1, TickerSymbol = "APPL", Price = 184.6m };
            var stock2 = new Stock() { Id = 2, TickerSymbol = "MSFT", Price = 324.3m };

            _repository.Setup(r => r.GetAllStocks()).ReturnsAsync(new List<Stock> { stock1, stock2 });

            var sut = CreateQueryHandler();

            // Act
            var result = await sut.Handle(new GetStocksQuery(Enumerable.Empty<string>()), CancellationToken.None);

            // Assert
            result.Count().Should().Be(2);
            result.Should().Contain(stock1);
            result.Should().Contain(stock2);
        }

        [Fact]
        public async Task HandleGetStocksQuery_IfTickerSymbolsProvided_ShouldReturnRequestedStocks()
        {
            // Arrange
            var stock1 = new Stock() { Id = 1, TickerSymbol = "APPL", Price = 184.6m };
            var stock2 = new Stock() { Id = 2, TickerSymbol = "MSFT", Price = 324.3m };
            var stock3 = new Stock() { Id = 3, TickerSymbol = "GOOGL", Price = 154.3m };

            _repository.Setup(r => r.GetStocksByTickerSymbol(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Stock> { stock2, stock3 });

            var sut = CreateQueryHandler();

            // Act
            var result = await sut.Handle(new GetStocksQuery(new List<string> { "MSFT", "GOOGL" }), CancellationToken.None);

            // Assert
            result.Count().Should().Be(2);
            result.Should().Contain(stock2);
            result.Should().Contain(stock3);
        }
    }
}
