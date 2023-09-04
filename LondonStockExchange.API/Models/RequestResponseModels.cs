using System.ComponentModel.DataAnnotations;

namespace LondonStockExchange.API.Models
{
    public record class GetStockResponse(Stock Stock);

    public record class GetStocksResponse(List<Stock> Stocks);

    public record class Stock(string TickerSymbol, decimal Price);

    public record class ReceiveTradeNotificationRequest
    {
        [Required]
        [MinLength(1), MaxLength(6)]
        public string TickerSymbol { get; init; }

        [Required]
        [Range(0.001, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; init; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Number of shares must be greater than zero.")]
        public double Shares { get; init; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Broker Id must be greater than zero.")]
        public int BrokerId { get; init; }
    };
}
