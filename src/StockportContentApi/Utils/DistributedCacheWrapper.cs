using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace StockportContentApi.Utils
{
    public interface IDistributedCacheWrapper
    {
        void Remove(string key);
        void SetString(string key, string value, DistributedCacheEntryOptions options);
        string GetString(string key);
    }

    public class DistributedCacheWrapper : IDistributedCacheWrapper
    {
        IDistributedCache _cache;

        public DistributedCacheWrapper(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string GetString(string key)
        {
            return _cache.GetString(key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void SetString(string key, string value, DistributedCacheEntryOptions options)
        {
            _cache.SetString(key, value, options);
        }
    }
}
