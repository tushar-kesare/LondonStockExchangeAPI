using LondonStockExchange.API.Models;
using LondonStockExchange.Domain.Commands;

namespace LondonStockExchange.API.Extensions;

public static class ModelMappingExtensions
{
    public static GetStockResponse ToResponseModel(this Domain.Entities.Stock stock)
    {
        return new GetStockResponse(new Stock(stock.TickerSymbol, stock.Price));
    }

    public static GetStocksResponse ToResponseModel(this IEnumerable<Domain.Entities.Stock> stocks)
    {
        return new GetStocksResponse(stocks.Select(s => new Stock(s.TickerSymbol, s.Price)).ToList());
    }

    public static CreateTradeCommand ToCreateTradeCommand(this ReceiveTradeNotificationRequest request, int stockId)
    {
        return new CreateTradeCommand(stockId, request.Price, request.Shares, request.BrokerId);
    }
}