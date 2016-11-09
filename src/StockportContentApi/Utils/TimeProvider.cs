using System;

namespace StockportContentApi.Utils
{
    public interface ITimeProvider
    {
        DateTime Now();
    }

    public class TimeProvider : ITimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
