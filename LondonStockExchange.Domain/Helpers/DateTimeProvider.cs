﻿namespace LondonStockExchange.Domain.Helpers
{
    public interface IDateTimeProvider
    {
        public DateTime UtcNow { get; }
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
