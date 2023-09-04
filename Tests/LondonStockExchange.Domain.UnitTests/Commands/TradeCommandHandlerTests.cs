using LondonStockExchange.Domain.Commands;
using LondonStockExchange.Domain.Entities;
using LondonStockExchange.Domain.Interfaces;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using LondonStockExchange.Domain.Helpers;
using Xunit;

namespace LondonStockExchange.Domain.UnitTests.Commands
{
    public class TradeCommandHandlerTests
    {
        private readonly Mock<IRepository> _repository = new Mock<IRepository>();
        private readonly Mock<IDateTimeProvider> _dateTimeProvider = new Mock<IDateTimeProvider>();
        private readonly DateTime _utcNow = DateTime.UtcNow;

        private TradeCommandHandler CreateCommandHandler()
        {
            return new TradeCommandHandler(_repository.Object, _dateTimeProvider.Object);
        }

        private void SetupMocks(DateTime utcNow)
        {
            _dateTimeProvider.Setup(p => p.UtcNow).Returns(utcNow);
        }

        [Fact]
        public async Task HandleCreateTradeCommand_ShouldAddTradeToRepository()
        {
            // Arrange
            var command = new CreateTradeCommand(62, 45.78m, 91.24, 3673);

            SetupMocks(_utcNow);

            var sut = CreateCommandHandler();

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            _repository.Verify(r => r.AddTrade(It.Is<Trade>(t => 
                t.TradeId != Guid.Empty
                && t.StockId == command.StockId
                && t.Price == command.Price
                && t.Shares == command.Shares
                && t.BrokerId == command.BrokerId
                && t.Timestamp == _utcNow
                )), Times.Once);
        }

        [Fact]
        public async Task HandleCreateTradeCommand_ShouldUpdateStockPrice()
        {
            // Arrange
            var command = new CreateTradeCommand(62, 45.78m, 91.24, 3673);

            SetupMocks(_utcNow);

            var sut = CreateCommandHandler();

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            _repository.Verify(r => r.UpdateStockPrice(command.StockId), Times.Once);
        }
    }
}
