namespace LondonStockExchange.Domain.Entities
{
    public class Stock
    {
        public int Id { get; set; }
        public string TickerSymbol { get; set; }
        public decimal Price { get; set; }
    }
}
