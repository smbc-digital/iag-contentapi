using System;
using System.Collections.Generic;
using System.Linq;
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
        string GetString(string key);
        List<RedisValueData> GetKeys();
    }

    public class DistributedCacheWrapper : IDistributedCacheWrapper
    {
        private string _redisIP;
        private readonly ILogger<IDistributedCacheWrapper> _logger;

        public DistributedCacheWrapper(string redisIP, ILogger<IDistributedCacheWrapper> logger)
        {
            _redisIP = redisIP;
            _logger = logger;
        }

        public string GetString(string key)
        {
            var cache = GetRedisCache();
            _logger.LogInformation($"Total Outstanding[GET]: {cache.GetCounters().TotalOutstanding}");
            var db = cache.GetDatabase();
            return db.StringGet(key);
        }

        public void Remove(string key)
        {
            var db = GetRedisCache().GetDatabase();
            db.KeyDelete(key);
        }

        public void SetString(string key, string value, int minutes)
        {
            var cache = GetRedisCache();
            _logger.LogInformation($"Total Outstanding[SET]: {cache.GetCounters().TotalOutstanding}");
            var db = cache.GetDatabase();
            db.StringAppend(key, value);
            db.KeyExpire(key, DateTime.Now.AddMinutes(minutes));
        }

        public List<RedisValueData> GetKeys()
        {
            var cache = GetRedisCache();
            _logger.LogInformation($"Total Outstanding[KEYS]: {cache.GetCounters().TotalOutstanding}");
            var db = cache.GetDatabase();
            var server = cache.GetServer(cache.Configuration);
            var keys = server.Keys();

            var redisValueData = new List<RedisValueData>();
            keys.Where(k => db.KeyType(k) == RedisType.String).ToList().ForEach(k =>
            {
                var valueWithExpiry = db.StringGetWithExpiry(k);

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

        private ConnectionMultiplexer GetRedisCache()
        {
            return ConnectionMultiplexer.Connect(_redisIP);

        }
    }
}
