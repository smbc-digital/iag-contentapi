using System;
using System.Collections.Generic;

namespace StockportContentApi.Config
{
    public class ContentfulConfig
    {
        public readonly string BusinessId;
        private readonly Dictionary<string, string> _config = new Dictionary<string, string>();

        public ContentfulConfig(string businessId)
        {
            Ensure.ArgumentNotNullOrEmpty(businessId, "BUSINESS_ID");
            BusinessId = businessId;
        }

        public Uri ContentfulUrl { get; private set; }

        public ContentfulConfig Add(string key, string value)
        {
            _config.Add(key, value);
            return this;
        }

        public ContentfulConfig Build()
        {
            var spaceKey = $"{BusinessId.ToUpper()}_SPACE";
            var acccessKey = $"{BusinessId.ToUpper()}_ACCESS_KEY";

            ContentfulUrl = new Uri($"{GetConfigValue("DELIVERY_URL")}/" +
                                    $"spaces/{GetConfigValue(spaceKey)}/" +
                                    $"entries?access_token={GetConfigValue(acccessKey)}");
            return this;
        }

        private string GetConfigValue(string key)
        {
            string value;
            if (_config.TryGetValue(key, out value))
                return value;
             throw new ArgumentException($"No value found for '{key}' in the contentful config.");
        }
    }
}