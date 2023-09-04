using LondonStockExchange.Domain.Entities;

namespace LondonStockExchange.Domain.Interfaces
{
    public interface IRepository
    {
        Task<IEnumerable<Stock>> GetAllStocks();

        Task<Stock> GetStockByTickerSymbol(string tickerSymbol);

        Task<IEnumerable<Stock>> GetStocksByTickerSymbol(IEnumerable<string> tickerSymbols);

        Task AddTrade(Trade trade);

        Task UpdateStockPrice(int stockId);
    }
}
