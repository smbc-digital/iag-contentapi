using System;

namespace StockportContentApi
{
    public static class Cache
    {
        public static readonly TimeSpan Short = TimeSpan.FromHours(12);
        public static readonly TimeSpan Medium = TimeSpan.FromHours(24);
        public static readonly TimeSpan Long = TimeSpan.FromHours(48);
    }
}
