using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using StockportContentApi.Model;

namespace StockportContentApi.Utils
{
    public interface IDistributedCacheWrapper
    {
        void Remove(string key);
        void SetString(string key, string value, int minutes);
        Task<string> GetString(string key);
        List<RedisValueData> GetKeys();
    }

    public class DistributedCacheWrapper : IDistributedCacheWrapper
    {
        private readonly string _redisIp;
        private readonly ILogger<IDistributedCacheWrapper> _logger;
        private readonly List<ConnectionMultiplexer> _cachePool;

        public DistributedCacheWrapper(string redisIp, ILogger<IDistributedCacheWrapper> logger)
        {
            _redisIp = redisIp;
            _logger = logger;
            _cachePool = new List<ConnectionMultiplexer>();
            for (var i = 0; i < 5; i++)
            {
                _cachePool.Add(ConnectionMultiplexer.Connect(redisIp + ",abortConnect=false"));
            }
        }

        public async Task<string> GetString(string key)
        {
            var db = GetLeastUsedConnection().GetDatabase();
            return await db.StringGetAsync(key);
        }

        public void Remove(string key)
        {
            var db = GetLeastUsedConnection().GetDatabase();
            db.KeyDeleteAsync(key);
        }
        
        public void SetString(string key, string value, int minutes)
        {
            var db = GetLeastUsedConnection().GetDatabase();
            db.StringSetAsync(key, value);
            db.KeyExpireAsync(key, DateTime.Now.AddMinutes(minutes));
        }

        public List<RedisValueData> GetKeys()
        {
            var cache = GetLeastUsedConnection();
            var db = cache.GetDatabase();
            var server = cache.GetServer(_redisIp);
            var keys = server.Keys();

            var redisValueData = new List<RedisValueData>();
            keys.Where(k => db.KeyTypeAsync(k).Result == RedisType.String).ToList().ForEach(async k =>
            {
                var valueWithExpiry = await db.StringGetWithExpiryAsync(k);

                var data = new RedisValueData
                {
                    NumberOfItems = 1,
                    Expiry = valueWithExpiry.Expiry.ToString(),
                    Key = k.ToString()
                };

                var jsonData = JsonConvert.DeserializeObject(valueWithExpiry.Value.ToString()) as JArray;

                if (jsonData != null) data.NumberOfItems = jsonData.Count;

                redisValueData.Add(data);
            });
            return redisValueData;
        }

        private ConnectionMultiplexer GetLeastUsedConnection()
        {
            var cache = _cachePool.OrderBy(c => c.GetCounters().TotalOutstanding).First();
            return cache;
        }
    }
}
