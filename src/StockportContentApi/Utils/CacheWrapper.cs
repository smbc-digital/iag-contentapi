using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace StockportContentApi.Utils
{
    public interface ICacheWrapper
    {
        void Set(string cacheKey, object cacheEntry, MemoryCacheEntryOptions cacheEntryOptions);
        bool TryGetValue(object key, out object value);
        
    }

    class CacheWrapper : ICacheWrapper
    {
        private IMemoryCache _memoryCache;

        public CacheWrapper(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Set(string cacheKey, object cacheEntry, MemoryCacheEntryOptions cacheEntryOptions)
        {
            _memoryCache.Set(cacheKey, cacheEntry, cacheEntryOptions);
        }

        public bool TryGetValue(object key, out object value)
        {
           return _memoryCache.TryGetValue(key, out value);
        }
    }
}
