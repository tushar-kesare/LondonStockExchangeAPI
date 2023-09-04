using LondonStockExchange.Domain.Extensions;
using LondonStockExchange.Domain.Helpers;
using LondonStockExchange.Domain.Interfaces;
using MediatR;

namespace LondonStockExchange.Domain.Commands;

public record class CreateTradeCommand(int StockId, decimal Price, double Shares, int BrokerId) : IRequest;

public class TradeCommandHandler : IRequestHandler<CreateTradeCommand>
{
    private readonly IRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TradeCommandHandler(IRepository repository, IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(CreateTradeCommand request, CancellationToken cancellationToken)
    {
        await _repository.AddTrade(request.ToTrade(_dateTimeProvider.UtcNow));

        await _repository.UpdateStockPrice(request.StockId);
    }
}