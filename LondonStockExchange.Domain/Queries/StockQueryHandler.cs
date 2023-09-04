using LondonStockExchange.Domain.Entities;
using LondonStockExchange.Domain.Interfaces;
using MediatR;

namespace LondonStockExchange.Domain.Queries;
public record class GetStockQuery(string TickerSymbol) : IRequest<Stock>;

public record class GetStocksQuery(IEnumerable<string> TickerSymbols) : IRequest<IEnumerable<Stock>>;

public class StockQueryHandler : IRequestHandler<GetStockQuery, Stock>,
    IRequestHandler<GetStocksQuery, IEnumerable<Stock>>
{
    private readonly IRepository _repository;

    public StockQueryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<Stock> Handle(GetStockQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetStockByTickerSymbol(request.TickerSymbol);
    }

    public async Task<IEnumerable<Stock>> Handle(GetStocksQuery request, CancellationToken cancellationToken)
    {
        if (!request.TickerSymbols.Any())
        {
            return await _repository.GetAllStocks();
        }

        return await _repository.GetStocksByTickerSymbol(request.TickerSymbols);
    }
}