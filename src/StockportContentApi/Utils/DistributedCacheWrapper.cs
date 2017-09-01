using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ConnectionMultiplexer _cache;
        private readonly IServer server;
        private readonly IDatabase db;


        public DistributedCacheWrapper(ConnectionMultiplexer cache)
        {
            _cache = cache;
            server = _cache.GetServer(_cache.Configuration);
            db = _cache.GetDatabase();
        }

        public string GetString(string key)
        {
            return db.StringGet(key);
        }

        public void Remove(string key)
        {
            db.KeyDelete(key);
        }

        public void SetString(string key, string value, int minutes)
        {
            db.StringAppend(key, value);
            db.KeyExpire(key, DateTime.Now.AddMinutes(minutes));
        }

        public List<RedisValueData> GetKeys()
        {
            var keys = server.Keys();

            var redisValueData = new List<RedisValueData>();
            keys.ToList().ForEach(k =>
            {
                var valueWithExpiry = db.StringGetWithExpiry(k);

                var data = new RedisValueData
                {
                    NumberOfItems = 1,
                    Expiry = valueWithExpiry.Expiry.ToString(),
                    Key = k.ToString()
                };

                var jsonData = JsonConvert.DeserializeObject(valueWithExpiry.Value.ToString()) as JArray;

                if(jsonData != null) data.NumberOfItems = jsonData.Count;

                redisValueData.Add(data);
            });
            return redisValueData;
        }
    }
}
