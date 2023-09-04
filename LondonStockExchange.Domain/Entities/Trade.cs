namespace LondonStockExchange.Domain.Entities;

public class Trade
{
    public Guid TradeId { get; set; }
    public int StockId { get; set; }
    public decimal Price { get; set; }
    public double Shares { get; set; }
    public int BrokerId { get; set; }
    public DateTime Timestamp { get; set; }
}
