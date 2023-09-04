using LondonStockExchange.Domain.Entities;
using LondonStockExchange.Domain.Interfaces;

namespace LondonStockExchange.Data
{
    public class InMemoryRepository : IRepository
    {
        private readonly List<Stock> _stocks = new List<Stock>
        {
            new Stock{Id = 1, TickerSymbol = "APPL"},
            new Stock{Id = 2, TickerSymbol = "GOOGL"},
            new Stock{Id = 3, TickerSymbol = "FB"},
            new Stock{Id = 4, TickerSymbol = "MSFT"},
            new Stock{Id = 5, TickerSymbol = "NVDA"},
        };

        private readonly List<Trade> _trades = new List<Trade>();

        public async Task<Stock> GetStockByTickerSymbol(string tickerSymbol)
        {
            return await Task.FromResult(_stocks.FirstOrDefault(s => s.TickerSymbol == tickerSymbol));
        }

        public async Task<IEnumerable<Stock>> GetStocksByTickerSymbol(IEnumerable<string> tickerSymbols)
        {
            return await Task.FromResult(_stocks.Where(s => tickerSymbols.Contains(s.TickerSymbol)));
        }

        public async Task<IEnumerable<Stock>> GetAllStocks()
        {
            return await Task.FromResult(_stocks);
        }

        public async Task UpdateStockPrice(int stockId)
        {
            var existingStock = _stocks.First(s => s.Id == stockId);

            existingStock.Price = _trades.Where(t => t.StockId== stockId)
                .Average(t => t.Price);

            await Task.CompletedTask;
        }

        public async Task AddTrade(Trade trade)
        {
            _trades.Add(trade);

            await Task.CompletedTask;
        }
    }
}