using LondonStockExchange.Domain.Commands;
using LondonStockExchange.Domain.Entities;

namespace LondonStockExchange.Domain.Extensions
{
    public static class EntityMappingExtensions
    {
        public static Trade ToTrade(this CreateTradeCommand command, DateTime timestamp)
        {
            return new Trade
            {
                TradeId = Guid.NewGuid(),
                StockId = command.StockId,
                Price = command.Price,
                Shares = command.Shares,
                BrokerId = command.BrokerId,
                Timestamp = timestamp
            };
        }
    }
}
