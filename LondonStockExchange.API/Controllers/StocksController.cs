using LondonStockExchange.API.Models;
using LondonStockExchange.Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using LondonStockExchange.API.Extensions;

namespace LondonStockExchange.API.Controllers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StocksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("/api/stocks/{tickerSymbol}")]
        [ProducesResponseType(typeof(GetStockResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetStock(string tickerSymbol)
        {
            var stock = await _mediator.Send(new GetStockQuery(tickerSymbol.ToUpper()));

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToResponseModel());
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetStocksResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetStocks([FromQuery]string? tickerSymbols)
        {
            var tickerSymbolsArray = string.IsNullOrEmpty(tickerSymbols) ? Array.Empty<string>() : tickerSymbols.Split(",");

            var stocks = await _mediator.Send(new GetStocksQuery(tickerSymbolsArray.Select(t => t.ToUpper())));

            return Ok(stocks.ToResponseModel());
        }
    }
}
