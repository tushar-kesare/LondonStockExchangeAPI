using LondonStockExchange.Domain.Entities;
using LondonStockExchange.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LondonStockExchange.API.UnitTests;

public class TestRepository : IRepository
{

    private readonly List<Stock> _stocks = new List<Stock>();

    private readonly List<Trade> _trades = new List<Trade>();

    public List<Stock> Stocks => _stocks;

    public List<Trade> Trades => _trades;

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

        existingStock.Price = _trades.Where(t => t.StockId == stockId)
            .Average(t => t.Price);

        await Task.CompletedTask;
    }

    public async Task AddTrade(Trade trade)
    {
        trade.TradeId = Guid.NewGuid();

        _trades.Add(trade);

        await Task.CompletedTask;
    }
}