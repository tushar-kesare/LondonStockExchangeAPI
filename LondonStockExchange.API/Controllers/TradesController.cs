using LondonStockExchange.API.Extensions;
using LondonStockExchange.API.Models;
using LondonStockExchange.Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LondonStockExchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TradesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveTradeNotification(ReceiveTradeNotificationRequest request)
        {
            var stock = await _mediator.Send(new GetStockQuery(request.TickerSymbol.ToUpper()));

            if (stock == null)
            {
                return UnprocessableEntity($"Invalid stock ticker symbol {request.TickerSymbol}");
            }

            await _mediator.Send(request.ToCreateTradeCommand(stock.Id));

            return Ok();
        }
    }
}
