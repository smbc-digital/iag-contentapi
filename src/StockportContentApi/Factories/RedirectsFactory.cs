using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StockportContentApi.Model;
using System.Linq;

namespace StockportContentApi.Factories
{
    public class RedirectsFactory : IFactory<BusinessIdToRedirects>
    {
        public BusinessIdToRedirects Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            if (entry == null || entry.fields == null) return new NullBusinessIdToRedirects();;

            var shortUrlRedirectDynamic = (IEnumerable<KeyValuePair<string, JToken>>)entry.fields.redirects;
            var shortUrlRedirectDictionary = shortUrlRedirectDynamic.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            var legacyUrlRedirectDynamic = (IEnumerable<KeyValuePair<string, JToken>>)entry.fields.legacyUrls;
            var legacyUrlRedirectDictionary = legacyUrlRedirectDynamic.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            return new BusinessIdToRedirects(shortUrlRedirectDictionary, legacyUrlRedirectDictionary);
        }
    }
}