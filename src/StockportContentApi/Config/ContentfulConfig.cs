using System;
using System.Collections.Generic;

namespace StockportContentApi.Config
{
    public class ContentfulConfig
    {
        public readonly string BusinessId;
        private readonly Dictionary<string, string> _config = new Dictionary<string, string>();
        public string SpaceKey;
        public string AccessKey;

        public ContentfulConfig(string businessId)
        {
            Ensure.ArgumentNotNullOrEmpty(businessId, "BUSINESS_ID");
            BusinessId = businessId;
        }

        public Uri ContentfulUrl { get; private set; }
        public Uri ContentfulContentTypesUrl { get; private set; }

        public ContentfulConfig Add(string key, string value)
        {
            _config.Add(key, value);
            return this;
        }

        public ContentfulConfig Build()
        {
            SpaceKey = GetConfigValue($"{BusinessId.ToUpper()}_SPACE");
            AccessKey = GetConfigValue($"{BusinessId.ToUpper()}_ACCESS_KEY");

            ContentfulUrl = new Uri($"{GetConfigValue("DELIVERY_URL")}/" +
                                    $"spaces/{SpaceKey}/" +
                                    $"entries?access_token={AccessKey}");

            ContentfulContentTypesUrl = new Uri($"{GetConfigValue("DELIVERY_URL")}/" +
                                    $"spaces/{SpaceKey}/" +
                                    $"content_types?access_token={AccessKey}");

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