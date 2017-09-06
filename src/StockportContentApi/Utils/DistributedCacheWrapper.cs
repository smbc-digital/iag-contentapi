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
        Task<List<RedisValueData>> GetKeys();
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
            _logger.LogInformation($"[GET] key: {key}");
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
            _logger.LogInformation($"[SET] key: {key}");
            var db = GetLeastUsedConnection().GetDatabase();
            db.StringSet(key, value);
            db.KeyExpire(key, DateTime.Now.AddMinutes(minutes));
        }

        public async Task<List<RedisValueData>> GetKeys()
        {
            var cache = GetLeastUsedConnection();
            var db = cache.GetDatabase();
            var server = cache.GetServer(_redisIp, 6379);
            var keys = server.Keys();

            _logger.LogInformation($"[GETKEYS]Total Keys: {keys.Count()}");
            _logger.LogInformation($"[GETKEYS]Cache Config: {cache.Configuration}");
            _logger.LogInformation($"[GETKEYS]Redis ip: {_redisIp}");

            var result = await GetRedisDataValueFromKey(keys, db);
            return result;
        }

        private async Task<List<RedisValueData>> GetRedisDataValueFromKey(IEnumerable<RedisKey> keys, IDatabase db)
        {
            var redisValueData = new List<RedisValueData>();

            foreach (var k in keys)
            {
                var keyType = await db.KeyTypeAsync(k);

                if (keyType != RedisType.String) continue;

                var valueWithExpiry = await db.StringGetWithExpiryAsync(k);
                _logger.LogInformation($"[GETKEYS] Key: {k} | Expiry:  {valueWithExpiry.Expiry}");

                var data = new RedisValueData
                {
                    NumberOfItems = 1,
                    Expiry = valueWithExpiry.Expiry.ToString(),
                    Key = k.ToString()
                };

                var jsonData = JsonConvert.DeserializeObject(valueWithExpiry.Value.ToString()) as JArray;

                if (jsonData != null) data.NumberOfItems = jsonData.Count;

                redisValueData.Add(data);
            }

            return redisValueData;
        }
        private ConnectionMultiplexer GetLeastUsedConnection()
        {
            var cache = _cachePool.OrderBy(c => c.GetCounters().TotalOutstanding).First();
            return cache;
        }
    }
}
